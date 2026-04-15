export const environment = {
  production: false,
  apiUrl: 'http://localhost:5220/api',
  api: 'http://localhost:5220',
  oauth: {
    googleCallbackUrl: 'http://localhost:4200/auth/callback/google',
    facebookCallbackUrl: 'http://localhost:4200/auth/callback/facebook',
  },
  tokenKey: {
    access: 'access_token',
    refresh: 'refresh_token',
  },
};
