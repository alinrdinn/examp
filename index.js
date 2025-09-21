// sso-proxy/index.js - FINAL FIX untuk DataHub Authentication
const express = require('express');
const jwt = require('jsonwebtoken');
const jwksClient = require('jwks-rsa');
const axios = require('axios');
const cookieParser = require('cookie-parser');
const session = require('express-session');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();
const PORT = process.env.PORT || 9003;

// Middleware
app.use(express.json());
app.use(cookieParser());

// Enhanced CORS middleware
app.use((req, res, next) => {
  const origin = req.headers.origin;
  const allowedOrigins = [
    'http://localhost:9003',
    'http://localhost:9002',
    'http://localhost:3000',
    'http://localhost:5236'
  ];
  
  if (allowedOrigins.includes(origin)) {
    res.setHeader('Access-Control-Allow-Origin', origin);
  } else {
    res.setHeader('Access-Control-Allow-Origin', '*');
  }
  
  res.setHeader('Access-Control-Allow-Credentials', 'true');
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
  res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization, X-Requested-With, X-DataHub-User, X-DataHub-User-Email, X-DataHub-User-ID, Cookie');
  
  if (req.method === 'OPTIONS') {
    res.status(200).end();
    return;
  }
  
  next();
});

app.use(session({
  secret: 'datahub-sso-secret',
  resave: false,
  saveUninitialized: false,
  cookie: { 
    secure: false,
    maxAge: 24 * 60 * 60 * 1000, // 24 hours
    httpOnly: false, // PENTING: Set false agar bisa diakses client-side
    sameSite: 'lax'
  },
  name: 'sso-session'
}));

// JWKS Client for JWT verification
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

// JWT Verification Function
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

// PENTING: Fungsi untuk check authentication
function isAuthenticated(req) {
  const sessionAuth = req.session && req.session.authenticated;
  const cookieAuth = req.cookies && req.cookies.datahub_authenticated === 'true';
  
  return sessionAuth && cookieAuth;
}

// ===== CRITICAL: DataHub Authentication Endpoints =====

// PENTING: DataHub memanggil endpoint ini untuk cek authentication
app.get('/authenticate/check', (req, res) => {
  console.log('🔍 /authenticate/check called');
  console.log('🔍 Cookies:', req.cookies);
  console.log('🔍 Session:', {
    sessionId: req.sessionID,
    authenticated: req.session.authenticated,
    user: req.session.user ? req.session.user.username : 'none'
  });
  
  if (isAuthenticated(req)) {
    console.log('✅ Authentication check SUCCESS');
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
    console.log('❌ Authentication check FAILED - redirecting to SSO');
    return res.status(401).json({ 
      authenticated: false,
      redirectUrl: '/sso/login?redirect_url=/'
    });
  }
});

// PENTING: DataHub current user endpoint
app.get('/api/v2/me', (req, res) => {
  console.log('🔍 /api/v2/me called');
  if (!isAuthenticated(req)) {
    console.log('❌ /api/v2/me - Not authenticated');
    return res.status(401).json({ 
      error: 'Not authenticated',
      redirectUrl: '/sso/login?redirect_url=/'
    });
  }

  console.log('✅ /api/v2/me - Returning user data');
  const user = req.session.user;
  return res.json({
    username: user.username,
    email: user.email,
    displayName: `${user.firstName} ${user.lastName}`,
    firstName: user.firstName,
    lastName: user.lastName,
    title: user.title || '',
    image: user.image || null,
    // PENTING: Privileges yang cukup untuk akses DataHub
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

// PENTING: DataHub config endpoints - CONFIGURE AUTHENTICATION TYPE
app.get('/config', (req, res) => {
  console.log('🔍 /config called');
  res.json({
    config: {
      authType: "OIDC", // PENTING: Set ke OIDC bukan INTERNAL
      isNativeUserCreationEnabled: false,
      isSSOEnabled: true,
      oidcEnabled: true, // PENTING: Enable OIDC
      jaasEnabled: false,
      viewsConfig: {
        enabled: true
      },
      // PENTING: Disable native login UI
      showBrowseV2: true,
      authConfig: {
        baseUrl: "http://localhost:9003"
      }
    }
  });
});

app.get('/api/v1/config', (req, res) => {
  console.log('🔍 /api/v1/config called');
  res.json({
    config: {
      authType: "OIDC", // PENTING: Set ke OIDC
      isNativeUserCreationEnabled: false,
      isSSOEnabled: true,
      oidcEnabled: true
    }
  });
});

// PENTING: Handle DataHub's /login endpoint
app.get('/login', (req, res) => {
  console.log('🔍 /login called with query:', req.query);
  
  // Jika sudah authenticated, redirect ke tujuan
  if (isAuthenticated(req)) {
    const redirectUrl = req.query.redirect_uri || '/';
    console.log('✅ Already authenticated, redirecting to:', redirectUrl);
    return res.redirect(redirectUrl);
  }
  
  // Jika belum authenticated, redirect ke SSO login
  const redirectUrl = req.query.redirect_uri || '/';
  const ssoLoginUrl = `/sso/login?redirect_url=${encodeURIComponent(redirectUrl)}`;
  console.log('🔒 Not authenticated, redirecting to SSO:', ssoLoginUrl);
  return res.redirect(ssoLoginUrl);
});

// DataHub logout endpoint
app.post('/logOut', (req, res) => {
  console.log('🔍 /logOut called');
  req.session.destroy((err) => {
    if (err) {
      console.error('❌ Logout error:', err);
      return res.status(500).json({ error: 'Logout failed' });
    }
    
    // Clear cookies
    res.clearCookie('PLAY_SESSION');
    res.clearCookie('datahub_authenticated');
    res.clearCookie('sso-session');
    
    console.log('✅ User logged out successfully');
    res.json({ success: true });
  });
});

// DataHub tracking endpoint
app.post('/track', (req, res) => {
  console.log('📊 Tracking request received');
  res.json({ success: true });
});

// ===== SSO LOGIN ENDPOINTS =====

// SSO Login via URL (for direct browser access)
app.get('/sso/login', async (req, res) => {
  try {
    const { sso_token, redirect_url = '/' } = req.query;
    
    if (!sso_token) {
      return res.status(400).send(`
        <html><body>
          <h2>Missing SSO Token</h2>
          <p>Please provide sso_token parameter</p>
          <p>Example: <code>/sso/login?sso_token=YOUR_JWT_TOKEN</code></p>
          <p><a href="/health">Check proxy status</a></p>
        </body></html>
      `);
    }

    // Verify JWT token
    const decoded = await verifyJWT(sso_token);
    
    // Extract user info
    const userInfo = {
      id: decoded.sub,
      username: decoded.preferred_username,
      email: decoded.email,
      firstName: decoded.given_name,
      lastName: decoded.family_name,
      roles: decoded.realm_access?.roles || [],
      token: sso_token
    };

    // Store in session
    req.session.user = userInfo;
    req.session.authenticated = true;
    req.session.authProvider = 'custom-sso';

    // PENTING: Save session dan tunggu completion
    req.session.save((err) => {
      if (err) {
        console.error('❌ Session save error:', err);
        return res.status(500).send(`
          <html><body>
            <h2>Session Error</h2>
            <p>Failed to save session: ${err.message}</p>
          </body></html>
        `);
      }

      // PENTING: Set cookies yang kompatibel dengan DataHub
      const cookieOptions = {
        httpOnly: false,
        secure: false,
        maxAge: 24 * 60 * 60 * 1000,
        sameSite: 'lax',
        domain: 'localhost'
      };

      // Session cookie untuk DataHub
      res.cookie('PLAY_SESSION', req.sessionID, cookieOptions);
      
      // Auth flag cookie
      res.cookie('datahub_authenticated', 'true', cookieOptions);
      
      // PENTING: Set additional DataHub cookies
      res.cookie('actor', userInfo.username, cookieOptions);

      console.log('✅ SSO Login successful for user:', userInfo.username);
      console.log('📋 Session ID:', req.sessionID);
      
      // Redirect dengan delay untuk memastikan cookies tersimpan
      setTimeout(() => {
        console.log(`🚀 Redirecting to: ${redirect_url}`);
        res.redirect(redirect_url);
      }, 100);
    });

  } catch (error) {
    console.error('❌ SSO Login Error:', error);
    res.status(401).send(`
      <html><body>
        <h2>Authentication Failed</h2>
        <p>Invalid or expired token</p>
        <pre>${error.message}</pre>
        <p><a href="/health">Check proxy status</a></p>
      </body></html>
    `);
  }
});

// SSO Login via POST (for .NET application)
app.post('/sso/login', async (req, res) => {
  try {
    const { access_token, redirect_url = '/' } = req.body;
    
    if (!access_token) {
      return res.status(400).json({ 
        success: false, 
        error: 'Missing access_token' 
      });
    }

    // Verify JWT token
    const decoded = await verifyJWT(access_token);
    
    // Extract user info
    const userInfo = {
      id: decoded.sub,
      username: decoded.preferred_username,
      email: decoded.email,
      firstName: decoded.given_name,
      lastName: decoded.family_name,
      roles: decoded.realm_access?.roles || [],
      token: access_token
    };

    // Store in session
    req.session.user = userInfo;
    req.session.authenticated = true;
    req.session.authProvider = 'custom-sso';
    
    // Save session
    req.session.save((err) => {
      if (err) {
        console.error('❌ Session save error:', err);
        return res.status(500).json({ success: false, error: 'Session save failed' });
      }
      
      // Set DataHub compatible cookies
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

      console.log('✅ SSO Login successful for user:', userInfo.username);

      res.json({
        success: true,
        message: 'SSO Login successful',
        user: {
          id: userInfo.id,
          username: userInfo.username,
          email: userInfo.email
        },
        redirect_url: redirect_url,
        session_id: req.sessionID
      });
    });

  } catch (error) {
    console.error('❌ SSO Login Error:', error);
    res.status(401).json({ 
      success: false, 
      error: 'Invalid or expired token' 
    });
  }
});

// User info endpoint
app.get('/sso/user', (req, res) => {
  if (!isAuthenticated(req)) {
    return res.status(401).json({ authenticated: false });
  }

  res.json({
    authenticated: true,
    user: {
      id: req.session.user.id,
      username: req.session.user.username,
      email: req.session.user.email,
      firstName: req.session.user.firstName,
      lastName: req.session.user.lastName,
      roles: req.session.user.roles,
      authProvider: req.session.authProvider
    },
    session: {
      id: req.sessionID,
      authenticated: req.session.authenticated
    }
  });
});

// ===== MIDDLEWARE UNTUK PROXY =====

const authMiddleware = (req, res, next) => {
  // System dan auth endpoints yang tidak perlu auth
  const skipAuthPaths = [
    '/health', '/api/v2/health', '/api/health',
    '/authenticate', '/authenticate/', '/authenticate/check', '/login',
    '/logOut', '/sso/', '/api/v2/me', '/track', '/config', '/api/v1/config'
  ];

  // Static assets
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

  // Check if should skip auth
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
    console.log(`🟡 Skipping auth for: ${req.method} ${req.path}`);
    return next();
  }

  // Check authentication
  if (!isAuthenticated(req)) {
    console.log(`🔒 Auth required for: ${req.method} ${req.path}`);
    
    // Untuk API calls, return JSON error
    if (req.path.startsWith('/api/') || req.headers.accept?.includes('application/json')) {
      return res.status(401).json({ 
        error: 'Not authenticated',
        redirectUrl: '/sso/login'
      });
    }
    
    // Untuk page requests, redirect ke SSO login
    return res.redirect(`/sso/login?redirect_url=${encodeURIComponent(req.originalUrl)}`);
  }

  // PENTING: Add user headers untuk DataHub
  if (req.session.user) {
    req.headers['X-DataHub-User'] = req.session.user.username;
    req.headers['X-DataHub-User-Email'] = req.session.user.email;
    req.headers['X-DataHub-User-ID'] = req.session.user.id;
    req.headers['X-DataHub-Auth-Provider'] = 'custom-sso';
    req.headers['X-DataHub-Actor'] = req.session.user.username;
  }

  console.log(`✅ Authenticated request for user ${req.session.user.username}: ${req.method} ${req.path}`);
  next();
};

// Direct asset proxy - NO authentication required
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
      console.error(`❌ Asset proxy error for ${req.originalUrl}:`, err.message);
      if (!res.headersSent) {
        res.status(404).send('Asset not found');
      }
    }
  })
);

// ===== MAIN PROXY untuk semua DataHub requests =====
app.use(
  '/', // Proxy semua request
  (req, res, next) => {
    // Skip jika sudah dihandle oleh endpoint lain
    if (req.path.startsWith('/sso/') || 
        req.path.startsWith('/health') ||
        req.path.startsWith('/debug/')) {
      return next('route'); // Skip proxy
    }
    next();
  },
  authMiddleware, // Apply auth middleware
  createProxyMiddleware({
    target: process.env.DATAHUB_FRONTEND_URL || 'http://datahub-frontend:9002',
    changeOrigin: true,
    ws: true,
    timeout: 60000,
    proxyTimeout: 60000,
    
    onProxyReq: (proxyReq, req, res) => {
      console.log(`📤 Proxy request: ${req.method} ${req.originalUrl}`);
      
      // PENTING: Forward semua cookies
      if (req.headers.cookie) {
        proxyReq.setHeader('Cookie', req.headers.cookie);
      }
      
      // PENTING: Tambahkan session cookie jika authenticated
      if (isAuthenticated(req) && req.sessionID) {
        const sessionCookie = `PLAY_SESSION=${req.sessionID}; actor=${req.session.user.username}`;
        const existingCookies = proxyReq.getHeader('Cookie');
        if (existingCookies) {
          proxyReq.setHeader('Cookie', `${existingCookies}; ${sessionCookie}`);
        } else {
          proxyReq.setHeader('Cookie', sessionCookie);
        }
      }
      
      // Add forwarding headers
      proxyReq.setHeader('X-Forwarded-For', req.ip);
      proxyReq.setHeader('X-Forwarded-Proto', req.protocol);
      proxyReq.setHeader('X-Forwarded-Host', req.get('Host'));
      proxyReq.setHeader('X-Real-IP', req.ip);
    },
    
    onProxyRes: (proxyRes, req, res) => {
      const path = req.path.toLowerCase();
      
      // Set correct content types
      if (path.endsWith('.css') || path.includes('.chunk.css')) {
        proxyRes.headers['content-type'] = 'text/css; charset=utf-8';
      } else if (path.endsWith('.js') || path.includes('.chunk.js')) {
        proxyRes.headers['content-type'] = 'application/javascript; charset=utf-8';
      }
      
      // CORS headers
      proxyRes.headers['Access-Control-Allow-Origin'] = '*';
      proxyRes.headers['Access-Control-Allow-Credentials'] = 'true';
      
      delete proxyRes.headers['x-frame-options'];
      delete proxyRes.headers['content-security-policy'];
      
      // Logging
      if (proxyRes.statusCode >= 400) {
        console.error(`❌ Proxy ERROR ${proxyRes.statusCode} for: ${req.originalUrl}`);
      } else if (proxyRes.statusCode === 200) {
        console.log(`✅ Proxy success: ${req.originalUrl}`);
      }
    },
    
    onError: (err, req, res) => {
      console.error(`❌ Proxy error for ${req.originalUrl}:`, err.message);
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

// Debug endpoint
app.get('/debug/auth', (req, res) => {
  const sessionAuth = req.session && req.session.authenticated;
  const cookieAuth = req.cookies && req.cookies.datahub_authenticated === 'true';
  
  res.json({
    timestamp: new Date().toISOString(),
    authentication: {
      sessionAuth: sessionAuth,
      cookieAuth: cookieAuth,
      isAuthenticated: isAuthenticated(req),
      sessionId: req.sessionID,
      user: req.session.user ? {
        username: req.session.user.username,
        email: req.session.user.email
      } : null
    },
    cookies: req.cookies,
    headers: {
      'user-agent': req.headers['user-agent'],
      'cookie': req.headers.cookie
    }
  });
});

// Health check
app.get('/health', (req, res) => {
  res.json({ 
    status: 'healthy',
    service: 'datahub-sso-proxy',
    version: '3.1.0',
    timestamp: new Date().toISOString(),
    authentication: {
      authenticated: isAuthenticated(req),
      session_id: req.sessionID,
      user: req.session.user ? req.session.user.username : null
    },
    endpoints: {
      'SSO Login': 'GET /sso/login?sso_token=JWT_TOKEN',
      'DataHub': 'GET /',
      'Auth Check': 'GET /authenticate/check',
      'User Info': 'GET /api/v2/me',
      'Config': 'GET /config'
    }
  });
});

// Error handling
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
  console.log(`🚀 DataHub SSO Proxy v3.1 running on port ${PORT}`);
  console.log(`📍 Key Features:`);
  console.log(`   - Handles DataHub /login redirects properly`);
  console.log(`   - Provides /authenticate/check endpoint`);
  console.log(`   - Returns proper user data via /api/v2/me`);
  console.log(`   - Configures DataHub as OIDC authentication`);
  console.log(`🔧 Endpoints:`);
  console.log(`   - SSO Login: http://localhost:${PORT}/sso/login?sso_token=JWT_TOKEN`);
  console.log(`   - DataHub: http://localhost:${PORT}/`);
  console.log(`   - Health: http://localhost:${PORT}/health`);
  console.log(`🔍 Expected Flow:`);
  console.log(`   1. User access http://localhost:${PORT}/`);
  console.log(`   2. If not authenticated → redirects to /sso/login`);
  console.log(`   3. After SSO login → redirects to DataHub dashboard`);
  console.log(`   4. DataHub calls /authenticate/check → gets authenticated=true`);
  console.log(`   5. DataHub shows dashboard ✅`);
});