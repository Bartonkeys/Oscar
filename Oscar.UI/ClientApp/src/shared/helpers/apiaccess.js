import { loginRequest } from "../../authConfig"
import { msalInstance } from "../../index";
import { toastr } from './toast';
import axios from 'axios'

const buildHeader = async (sendAsForm = false) => {
  const accounts = msalInstance.getAllAccounts();
  const token = await msalInstance.acquireTokenSilent({
      ...loginRequest,
      account: accounts[0]
  });

  const bearer = `Bearer ${token.accessToken}`;
  const headers = {
    'Authorization': bearer,
    'Content-Type': sendAsForm? 'multipart/form-data' : 'application/json'
  };
  return headers;
}

export const showValidationError = (message) => {
  const Msg = () => (
    <ul style={{textAlign: 'left'}}>
      {message.split('\n').map((str, index) => (<li key={index}>{str}</li>))}
    </ul>
  );
  toastr('error' , <Msg />);
}

export const search = async (url, fetchOptions, searchOptions, searchByQueryString) => {
  const {sortColumn, sortDirection, start, take} = fetchOptions;
  const config = {
    headers: await buildHeader(),
  };

  let response;
  if(searchByQueryString){
    url = `${url}?`;
    url += `start=${start}&`;
    url += `take=${take}&`;
    Object.keys(searchByQueryString).forEach((query) => {
      url += `${query}=${searchByQueryString[query]}&` 
    });
    response = await axios.get(url, config);
  }
  else{
    const {searchObjects} = searchOptions;
    const postBody = {
        sortColumn,
        sortDirection: sortDirection === 'asc' ? "ascending" : "descending",
        searchObjects,
        Start: start,
        Take: take
    }
    response = await axios.post(url, postBody, config);
  } 
  return response.data;
};

export const get = async (url) => {
  const config = {
      headers: await buildHeader()
  };
  const response = await axios.get(url, config);
  return response.data;
};

export const create = async (url, body, postForm=false) => {
    const config = {
        headers: postForm? await buildHeader(true): await buildHeader(),
    };
    if(postForm){
      const formData = new FormData();
      for (const item in body){
        if(body[item]){
          formData.append(item, body[item]);
        }
      }
      formData.append('jb', 'test');
      return await axios.post(url, formData, config); 
    }
    return await axios.post(url, body, config);
};

export const update = async (url, body) => {
  const config = {
      headers: await buildHeader(),
  };
  return await axios.put(url, body, config);
};

export const remove = async (url) => {
  const config = {
      headers: await buildHeader()
  };
  const response = await axios.delete(url, config);
  return response.data;
};