const express = require('express');
const jwt = require('jsonwebtoken');
const jwksClient = require('jwks-rsa');
const cookieParser = require('cookie-parser');
const session = require('express-session');
const { createProxyMiddleware } = require('http-proxy-middleware');
const cors = require('cors');

const app = express();
const PORT = process.env.PORT || 9003;

app.use(express.json());
app.use(cookieParser());
app.use(cors({
  origin: (origin, callback) => {
    if (!origin) return callback(null, true);

    const allowedOrigins = [
      'http://localhost:9003',
      'http://localhost:9002',
      'http://localhost:3000',
      'http://localhost:5236'
    ];
    if (allowedOrigins.indexOf(origin) === -1) {
      const msg = 'The CORS policy for this site does not allow access from the specified Origin.';
      return callback(new Error(msg), false);
    }
    return callback(null, true);
  },
  credentials: true,
}));

app.use(session({
  secret: process.env.SESSION_SECRET || 'datahub-sso-secret',
  resave: false,
  saveUninitialized: false,
  cookie: { 
    secure: process.env.NODE_ENV === 'production',
    maxAge: 24 * 60 * 60 * 1000,
    httpOnly: true,
    sameSite: 'lax'
  },
  name: 'sso-session'
}));

const client = jwksClient({
  jwksUri: process.env.JWKS_URI || 'http://keycloak-for-datahub:8180/realms/auth-party/protocol/openid-connect/certs'
});

function getKey(header, callback) {
  client.getSigningKey(header.kid, (err, key) => {
    if (err) {
      return callback(err, null);
    }
    const signingKey = key.publicKey || key.rsaPublicKey;
    callback(null, signingKey);
  });
}

function verifyJWT(token) {
  return new Promise((resolve, reject) => {
    jwt.verify(token, getKey, {
      audience: process.env.JWT_AUDIENCE || 'public-client-auth-dotnet',
      issuer: process.env.JWT_ISSUER || 'http://localhost:8180/realms/auth-party',
      algorithms: ['RS256']
    }, (err, decoded) => {
      if (err) {
        reject(err);
      } else {
        resolve(decoded);
      }
    });
  });
}

function isAuthenticated(req) {
  return !!(req.session && req.session.authenticated);
}

app.get('/authenticate/check', (req, res) => {
  if (isAuthenticated(req)) {
    return res.json({
      authenticated: true,
      user: {
        username: req.session.user.username,
        email: req.session.user.email,
        displayName: `${req.session.user.firstName} ${req.session.user.lastName}`,
        privileges: ["MANAGE_USERS", "MANAGE_POLICIES", "MANAGE_DOMAINS", "MANAGE_INGESTION"]
      }
    });
  } else {
    return res.status(401).json({
      authenticated: false,
      redirectUrl: '/sso/login?redirect_url=/'
    });
  }
});

app.get('/api/v2/me', (req, res) => {
  if (!isAuthenticated(req)) {
    return res.status(401).json({ 
      error: 'Not authenticated',
      redirectUrl: '/sso/login?redirect_url=/'
    });
  }

  const user = req.session.user;
  return res.json({
    username: user.username,
    email: user.email,
    displayName: `${user.firstName} ${user.lastName}`,
    firstName: user.firstName,
    lastName: user.lastName,
    title: user.title || '',
    image: user.image || null,
    privileges: [
      "MANAGE_USERS", 
      "MANAGE_POLICIES", 
      "MANAGE_DOMAINS", 
      "MANAGE_INGESTION",
      "MANAGE_SECRETS",
      "MANAGE_GLOSSARIES",
      "MANAGE_TAGS",
      "VIEW_ANALYTICS"
    ],
    corpUser: {
      username: user.username,
      info: {
        email: user.email,
        fullName: `${user.firstName} ${user.lastName}`,
        firstName: user.firstName,
        lastName: user.lastName,
        displayName: `${user.firstName} ${user.lastName}`
      }
    }
  });
});

app.get('/config', (req, res) => {
  res.json({
    config: {
      authType: "OIDC",
      isNativeUserCreationEnabled: false,
      isSSOEnabled: true,
      oidcEnabled: true,
      jaasEnabled: false,
      viewsConfig: {
        enabled: true
      },
      showBrowseV2: true,
      authConfig: {
        baseUrl: "http://localhost:9003"
      }
    }
  });
});

app.get('/api/v1/config', (req, res) => {
  res.json({
    config: {
      authType: "OIDC",
      isNativeUserCreationEnabled: false,
      isSSOEnabled: true,
      oidcEnabled: true
    }
  });
});

app.get('/login', (req, res) => {
  if (isAuthenticated(req)) {
    const redirectUrl = req.query.redirect_uri || '/';
    return res.redirect(redirectUrl);
  }
  
  const redirectUrl = req.query.redirect_uri || '/';
  const ssoLoginUrl = `/sso/login?redirect_url=${encodeURIComponent(redirectUrl)}`;
  return res.redirect(ssoLoginUrl);
});

app.post('/logOut', (req, res) => {
  req.session.destroy((err) => {
    if (err) {
      console.error('Logout error:', err);
      return res.status(500).json({ error: 'Logout failed' });
    }
    
    res.clearCookie('PLAY_SESSION');
    res.clearCookie('datahub_authenticated');
    res.clearCookie('sso-session');
    
    res.json({ success: true });
  });
});

app.post('/track', (req, res) => {
  res.json({ success: true });
});

app.get('/sso/login', async (req, res) => {
  try {
    const { sso_token, redirect_url = '/' } = req.query;
    
    if (!sso_token) {
      return res.status(400).send('SSO token is missing.');
    }
    const decoded = await verifyJWT(sso_token);
    
    const userInfo = {
      id: decoded.sub,
      username: decoded.preferred_username,
      email: decoded.email,
      firstName: decoded.given_name,
      lastName: decoded.family_name,
      roles: decoded.realm_access?.roles || [],
      token: sso_token
    };

    req.session.user = userInfo;
    req.session.authenticated = true;
    req.session.authProvider = 'custom-sso';

    req.session.save((err) => {
      if (err) {
        console.error('Session save error:', err);
        return res.status(500).send('Failed to save session.');
      }
      const cookieOptions = {
        httpOnly: false,
        secure: false,
        maxAge: 24 * 60 * 60 * 1000,
        sameSite: 'lax',
        domain: 'localhost'
      };

      res.cookie('PLAY_SESSION', req.sessionID, cookieOptions);
      
      res.cookie('datahub_authenticated', 'true', cookieOptions);
      
      res.cookie('actor', userInfo.username, cookieOptions);
      
      res.redirect(redirect_url);
    });

  } catch (error) {
    console.error('SSO Login Error:', error);
    res.status(401).send('Authentication failed. Invalid or expired token.');
  }
});


const authMiddleware = (req, res, next) => {
  const skipAuthPaths = [
    '/health', '/api/v2/health', '/api/health',
    '/authenticate', '/authenticate/', '/authenticate/check', '/login',
    '/logOut', '/sso/', '/api/v2/me', '/track', '/config', '/api/v1/config'
  ];

  const staticPatterns = [
    '/assets/', '/static/', '/public/', '/js/', '/css/', '/images/', '/img/',
    '/fonts/', '/font/', '/icons/', '/media/', '/_next/', '/webpack/',
    '/__webpack', '/build/', '/dist/', '/favicon.ico', '/manifest.json',
    '/robots.txt', '/sitemap.xml'
  ];

  const staticExtensions = [
    '.css', '.js', '.ts', '.jsx', '.tsx', '.png', '.jpg', '.jpeg', '.gif',
    '.svg', '.ico', '.webp', '.avif', '.woff', '.woff2', '.ttf', '.eot',
    '.otf', '.json', '.xml', '.txt', '.map', '.wasm', '.pdf', '.zip'
  ];

  const shouldSkipAuth = 
    skipAuthPaths.some(path => req.path.startsWith(path)) ||
    staticPatterns.some(pattern => req.path.startsWith(pattern)) ||
    staticExtensions.some(ext => req.path.toLowerCase().endsWith(ext)) ||
    req.path.includes('.chunk.') ||
    req.path.includes('.bundle.') ||
    req.headers.accept?.includes('text/css') ||
    req.headers.accept?.includes('application/javascript') ||
    req.headers.accept?.includes('image/');

  if (shouldSkipAuth) {
    return next();
  }

  if (!isAuthenticated(req)) {
    if (req.path.startsWith('/api/') || req.headers.accept?.includes('application/json')) {
      return res.status(401).json({ 
        error: 'Not authenticated',
        redirectUrl: '/sso/login'
      });
    }
    
    return res.redirect(`/sso/login?redirect_url=${encodeURIComponent(req.originalUrl)}`);
  }

  if (req.session.user) {
    req.headers['X-DataHub-User'] = req.session.user.username;
    req.headers['X-DataHub-User-Email'] = req.session.user.email;
    req.headers['X-DataHub-User-ID'] = req.session.user.id;
    req.headers['X-DataHub-Auth-Provider'] = 'custom-sso';
    req.headers['X-DataHub-Actor'] = req.session.user.username;
  }

  next();
};

app.use(
  ['/assets', '/static', '/public', '/js', '/css', '/images', '/fonts'],
  createProxyMiddleware({
    target: process.env.DATAHUB_FRONTEND_URL || 'http://datahub-frontend:9002',
    changeOrigin: true,
    ws: false,
    timeout: 30000,
    proxyTimeout: 30000,
    
    onProxyRes: (proxyRes, req, res) => {
      const path = req.path.toLowerCase();
      if (path.endsWith('.css')) {
        proxyRes.headers['content-type'] = 'text/css; charset=utf-8';
      } else if (path.endsWith('.js')) {
        proxyRes.headers['content-type'] = 'application/javascript; charset=utf-8';
      }
      
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Cache-Control'] = 'public, max-age=31536000';
      
      delete proxyRes.headers['x-frame-options'];
      delete proxyRes.headers['content-security-policy'];
    },
    
    onError: (err, req, res) => {
      console.error(`Asset proxy error for ${req.originalUrl}:`, err.message);
      if (!res.headersSent) {
        res.status(404).send('Asset not found');
      }
    }
  })
);

app.use(
  '/',
  (req, res, next) => {
    if (req.path.startsWith('/sso/') || 
        req.path.startsWith('/health') ||
        req.path.startsWith('/debug/')) {
      return next('route');
    }
    next();
  },
  authMiddleware,
  createProxyMiddleware({
    target: process.env.DATAHUB_FRONTEND_URL || 'http://datahub-frontend:9002',
    changeOrigin: true,
    ws: true,
    timeout: 60000,
    proxyTimeout: 60000,
    
    onProxyReq: (proxyReq, req, res) => {
      if (req.headers.cookie) {
        proxyReq.setHeader('Cookie', req.headers.cookie);
      }
      
      if (isAuthenticated(req) && req.sessionID) {
        const sessionCookie = `PLAY_SESSION=${req.sessionID}; actor=${req.session.user.username}`;
        const existingCookies = proxyReq.getHeader('Cookie');
        if (existingCookies) {
          proxyReq.setHeader('Cookie', `${existingCookies}; ${sessionCookie}`);
        } else {
          proxyReq.setHeader('Cookie', sessionCookie);
        }
      }

      proxyReq.setHeader('X-Forwarded-For', req.ip);
      proxyReq.setHeader('X-Forwarded-Proto', req.protocol);
      proxyReq.setHeader('X-Forwarded-Host', req.get('Host'));
      proxyReq.setHeader('X-Real-IP', req.ip);
    },
    
    onProxyRes: (proxyRes, req, res) => {
      const path = req.path.toLowerCase();
      
      if (path.endsWith('.css') || path.includes('.chunk.css')) {
        proxyRes.headers['content-type'] = 'text/css; charset=utf-8';
      } else if (path.endsWith('.js') || path.includes('.chunk.js')) {
        proxyRes.headers['content-type'] = 'application/javascript; charset=utf-8';
      }
      
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Access-Control-Allow-Credentials'] = 'true';
      
      delete proxyRes.headers['x-frame-options'];
      delete proxyRes.headers['content-security-policy'];
    },
    
    onError: (err, req, res) => {
      console.error(`Proxy error for ${req.originalUrl}:`, err.message);
      if (!res.headersSent) {
        res.status(500).json({
          error: 'Proxy Error',
          message: 'Failed to connect to DataHub frontend',
          path: req.originalUrl
        });
      }
    }
  })
);

app.use((error, req, res, next) => {
  console.error('❌ Global Error:', error);
  if (!res.headersSent) {
    res.status(500).json({ 
      error: 'Internal server error',
      message: error.message 
    });
  }
});

app.listen(PORT, '0.0.0.0', () => {
  console.log(`DataHub SSO Proxy is running on http://localhost:${PORT}`);
});