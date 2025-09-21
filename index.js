const express = require('express');
const cookieParser = require('cookie-parser');
const { createProxyMiddleware } = require('http-proxy-middleware');
const jwt = require('jsonwebtoken');
const jwksRsa = require('jwks-rsa');
const https = require('https');

const PORT = 9003;
const DATAHUB_TARGET = 'http://localhost:9002';
const LOGIN_PATH = '/sso/login';
const TOKEN_COOKIE = 'datahub_proxy_jwt';
const ACTOR_COOKIE = 'datahub_proxy_actor';
const SECURE_COOKIES = false;
const ACTOR_CLAIM = null;

const KEYCLOAK_BASE_URL = 'https://10.54.18.146/nic/authentication';
const KEYCLOAK_REALM_NAME = 'SQM';
const KEYCLOAK_CLIENT_ID = 'sqm-auth-service';
const KEYCLOAK_ISSUER = `${KEYCLOAK_BASE_URL}/realms/${KEYCLOAK_REALM_NAME}`;
const JWT_ALGORITHMS = ['RS256'];

const httpsAgent = new https.Agent({ rejectUnauthorized: false });
const jwksClient = jwksRsa({
  jwksUri: `${KEYCLOAK_ISSUER}/protocol/openid-connect/certs`,
  requestAgent: httpsAgent,
});

const app = express();

app.use(express.json());
app.use(cookieParser());

const getSigningKey = async (token) => {
  const decoded = jwt.decode(token, { complete: true });

  if (!decoded || !decoded.header?.kid) {
    throw new Error('Token header does not contain a key identifier');
  }

  const signingKey = await jwksClient.getSigningKey(decoded.header.kid);
  if (typeof signingKey.getPublicKey === 'function') {
    return signingKey.getPublicKey();
  }

  return signingKey.publicKey || signingKey.rsaPublicKey;
};

const verifyToken = async (token) => {
  if (!token) {
    throw new Error('Missing token');
  }

  const publicKey = await getSigningKey(token);

  return jwt.verify(token, publicKey, {
    algorithms: JWT_ALGORITHMS,
    audience: KEYCLOAK_CLIENT_ID,
    issuer: KEYCLOAK_ISSUER,
    ignoreExpiration: true,
  });
};

const resolveActor = (claims) => {
  if (!claims) {
    return null;
  }

  const desiredClaim = ACTOR_CLAIM;
  if (desiredClaim && claims[desiredClaim]) {
    return claims[desiredClaim];
  }

  return (
    claims.preferred_username ||
    claims.email ||
    claims.sub ||
    claims.name ||
    null
  );
};

const buildLoginRedirect = (req) => {
  const redirect = encodeURIComponent(req.originalUrl || '/');
  return `${LOGIN_PATH}?redirect_url=${redirect}`;
};

const clearAuthCookies = (res) => {
  res.clearCookie(TOKEN_COOKIE);
  res.clearCookie(ACTOR_COOKIE);
};

const requireAuthentication = async (req, res, next) => {
  if (req.path.startsWith(LOGIN_PATH) || req.path === '/health') {
    return next();
  }

  const token = req.cookies[TOKEN_COOKIE];

  if (!token) {
    const loginLocation = buildLoginRedirect(req);

    if (req.accepts('json')) {
      return res.status(401).json({ error: 'Unauthorized', login: loginLocation });
    }

    return res.redirect(loginLocation);
  }

  try {
    const claims = await verifyToken(token);
    const actor = resolveActor(claims);

    if (!actor) {
      throw new Error('Token does not contain an actor claim');
    }

    req.auth = { token, claims, actor };
    return next();
  } catch (error) {
    clearAuthCookies(res);

    const loginLocation = buildLoginRedirect(req);

    if (req.accepts('json')) {
      return res.status(401).json({
        error: 'Unauthorized',
        message: error.message,
        login: loginLocation,
      });
    }

    return res.redirect(loginLocation);
  }
};

app.get('/health', (req, res) => {
  res.json({ status: 'ok', proxy: 'datahub', timestamp: new Date().toISOString() });
});

app.get(LOGIN_PATH, async (req, res) => {
  const token = req.query.token || req.query.jwt || req.query.sso_token;
  const redirectUrl = req.query.redirect_url || '/';

  if (!token) {
    return res.status(400).json({ error: 'Missing token parameter' });
  }

  try {
    const claims = await verifyToken(token);
    const actor = resolveActor(claims);

    if (!actor) {
      throw new Error('Token does not contain an actor claim');
    }

    const cookieOptions = {
      httpOnly: true,
      sameSite: 'lax',
      secure: SECURE_COOKIES,
      maxAge: 24 * 60 * 60 * 1000,
    };

    res.cookie(TOKEN_COOKIE, token, cookieOptions);
    res.cookie(
      ACTOR_COOKIE,
      actor,
      { ...cookieOptions, httpOnly: false }
    );

    return res.redirect(redirectUrl);
  } catch (error) {
    return res.status(401).json({ error: 'Invalid token', message: error.message });
  }
});

app.use(
  requireAuthentication,
  createProxyMiddleware({
    target: DATAHUB_TARGET,
    changeOrigin: true,
    ws: true,
    onProxyReq: (proxyReq, req) => {
      if (req.auth) {
        proxyReq.setHeader('Authorization', `Bearer ${req.auth.token}`);
        proxyReq.setHeader('X-DataHub-Actor', req.auth.actor);
        proxyReq.setHeader('X-DataHub-Claims', Buffer.from(JSON.stringify(req.auth.claims)).toString('base64'));
      }
    },
  })
);

app.listen(PORT, () => {
  // eslint-disable-next-line no-console
  console.log(`DataHub proxy listening on port ${PORT}`);
});
