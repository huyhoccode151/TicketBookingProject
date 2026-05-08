const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7062';

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
    ],
    target,
    secure: false,
    onProxyRes: function (proxyRes) {
      proxyRes.headers['cross-origin-opener-policy'] = 'same-origin-allow-popups';
    }

  },
    {
      context:
        ["/api"],
      "target": "http://localhost:5220",
      "secure": false,
      "headers": {
        "Cross-Origin-Opener-Policy": "same-origin-allow-popups"
      }
  }
]

module.exports = PROXY_CONFIG;
