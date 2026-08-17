import Axios from 'axios';
import { setErrorMessage } from '../../App';
import history from './history';
import qs from 'qs';

let authToken;
let userRole = '';
let clientName = '';
let canUserSelectClients = false;
let mritUrl = '';
let affinityUrl = '';

export function getClientName() {
  return clientName || '';
}

export function canEdit() {
  return userRole === 'SysAdmin' || userRole === 'Editor';
}

export function canSeeOps() {
  return userRole === 'SysAdmin' || userRole === 'Editor' || userRole === 'Compact';
}

export function isAdmin() {
  return userRole === 'SysAdmin';
}

export function isCompactCollections() {
  return userRole === 'CompactCollections';
}

export function isCompactSales() {
  return userRole === 'Compact';
}

export function canSelectClient() {
  return canUserSelectClients;
}

function getBaseURL() {
  let baseURL = window.location.href.match(/^http(s)?:\/\/[^/]+\//g)[0];
  baseURL = baseURL.substring(0, baseURL.length - 1);
  return baseURL;
}

export let authClient = Axios.create({
  baseURL: getBaseURL(),
  paramsSerializer: function (params) {
    return qs.stringify(params, { arrayFormat: 'repeat' })
  }
});

function responseErrorInterceptor(error) {
  if (Axios.isCancel(error)) {
    return Promise.reject(error);
  }

  if (error.response && error.response.status === 401) {
    if (!error.response.config.url.startsWith('api/auth')) {
      logout();
    }
    else if (!window.location.href.endsWith('/login')) {
      history.push('/login');
    }
  }
  else if (error.response && error.response.config.url.startsWith('api/auth/refresh/token')) {
    history.push('/login');
  }
  else if (error.response.data.errorMessage) {
    setErrorMessage(error.response.data.errorMessage);
  }
  else if (error.response.data.errors) {
    setErrorMessage(Object.values(error.response.data.errors).join(' '))
  }
  return Promise.reject(error);
};

async function requestInterceptor(config) {
  if (!config.url.startsWith('api/auth')) {
    if (authToken) {
      if ((new Date()).getTime() > authToken.expiry) {
        await refresh();
      }

      setHeaders(config);
    }
    else {
      await refresh();
      setHeaders(config);
    }
  }
  return config;
};

let lazyMritClient;
export function getMritClient() {
  if(!lazyMritClient) {
    lazyMritClient = Axios.create({
      baseURL: mritUrl,
      paramsSerializer: function (params) {
        return qs.stringify(params, { arrayFormat: 'repeat' })
      }
    });

    lazyMritClient.interceptors.response.use(response => {
      return response;
    }, responseErrorInterceptor);
    
    lazyMritClient.interceptors.request.use(requestInterceptor);
  }

  return lazyMritClient;
}

let lazyAffinityClient;
export function getAffinityClient() {  
  if(!lazyAffinityClient) {
    
    lazyAffinityClient = Axios.create({
      baseURL: affinityUrl,
      paramsSerializer: function (params) {
        return qs.stringify(params, { arrayFormat: 'repeat' })
      }
    });    

    lazyAffinityClient.interceptors.response.use(response => {
      return response;
    }, responseErrorInterceptor);
    
    lazyAffinityClient.interceptors.request.use(requestInterceptor);    
  }
  return lazyAffinityClient;
}

let cancellationTokens = {};
export function getCancellationToken(name) {
  let source = Axios.CancelToken.source();
  cancellationTokens[name] = source;

  return source.token;
}

export function cancelRequest(name) {
  let source = cancellationTokens[name];
  if (source) {
    source.cancel();
  }
}

setInterval(() => {
  if (authToken && ((new Date()).getTime() + 2 * 60000) >= authToken.expiry) {
    refresh();
  }
}, 60000);

authClient.interceptors.response.use(response => {
  return response;
}, responseErrorInterceptor);

authClient.interceptors.request.use(requestInterceptor);

function setHeaders(config) {
  config.url = 'api/' + config.url;

  if (!authToken) {
    throw new Axios.Cancel('Operation canceled');
  }

  config.headers.common = { 'Authorization': `bearer ${authToken.token}` };
}

export function login(data) {
  authToken = {
    token: data.token,
    expiry: data.expiryTime * 1000
  };
  userRole = data.role;
  clientName = data.clientName;
  canUserSelectClients = data.canSelectClients;
  mritUrl = data.mritUrl;
  affinityUrl = data.affinityUrl;

  if (userRole === "ExternalAccess") {
    logout();
  }
}

export async function refresh() {
  try {
    var response = await authClient.post('api/auth/refresh/token')
      .catch(err => {
        throw new Error("Something went wrong. Logout.");
      });
    login(response.data);
    return true;
  }
  catch (error) {
    logout();
    return false;
  }
}

export function logout() {
  authToken = null;
  if (!window.location.href.endsWith('/login')) {
    authClient.post('api/auth/logout').then(response => {
      history.push('/login');
    });
  }
}

export async function isLoggedIn() {
  if (authToken) {

    if ((new Date()).getTime() > authToken.expiry) {
      return await refresh();
    }

  }
  else {
    return await refresh();
  }

  return true;
}

export let AFFINITY = 'AFFINITY';
export let MRIT = 'MRIT';
export let AUTH = 'AUTH';

export function getClientType(name) {
  if(name === AFFINITY) {
    return getAffinityClient();
  }
  else if(name === MRIT) {
    return getMritClient();
  }
  else if(name === 'AUTH') {
    return authClient;
  }

  return null;
}