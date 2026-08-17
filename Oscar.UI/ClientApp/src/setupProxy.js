const createProxyMiddleware = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:5188` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5188'; //Or Maybe  'http://localhost:4159'

const context =  [
    "/client",
    "/works",
    "/staticData",
    "/matchRequest",
    "/series",
    "/season",
    "/episode",
    "/standAlone",
    "/actor",
    "/director",
    "/producer",
    "/screenWriter",
];

module.exports = function(app) {
  const appProxy = createProxyMiddleware(context, {
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  });

  app.use(appProxy);
};
