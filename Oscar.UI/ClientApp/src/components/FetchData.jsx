import React, { useState, useEffect } from 'react';
import { useMsal, AuthenticatedTemplate, UnauthenticatedTemplate, useIsAuthenticated } from "@azure/msal-react";
import { loginRequest } from "../authConfig"

export const FetchData = (props) => {
  const [clients, setClients] = useState(null);
  const [loading, setLoading] = useState(true);
  const { instance, accounts } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  const populateClientData = async () =>
  {
      if(isAuthenticated){
          const token = await instance.acquireTokenSilent({
            ...loginRequest,
            account: accounts[0]
          });
    
          const headers = new Headers();
          const bearer = `Bearer ${token.accessToken}`;
    
          headers.append("Authorization", bearer);
    
          const options = {
              method: "GET",
              headers: headers
          };
    
          var response = await fetch('client', options);
          const data = await response.json();
          setClients(data);
          setLoading(false);
      }
    }

  useEffect(() => {
     populateClientData();
  }, []);

  return(
    <div>
        <h1 id="tabelLabel">Clients</h1>
        {clients ? 
      <><AuthenticatedTemplate>
          <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>Id</th>
                <th>Client Reference</th>
                <th>Client Name</th>
                <th>Email</th>
                <th>Status</th>
                <th>Grade</th>
              </tr>
            </thead>
            <tbody>
              {clients.map(client => <tr key={client.id}>
                <td>{client.id}</td>
                <td>{client.clientReference}</td>
                <td>{client.clientName}</td>
                <td>{client.email}</td>
                <td>{client.status}</td>
                <td>{client.clientGrade}</td>
              </tr>
              )}
            </tbody>
          </table>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <h5 className="card-title">Please sign-in to see client information.</h5>
        </UnauthenticatedTemplate></>
    : <h2>Loading...</h2>}

    </div>
  );

}