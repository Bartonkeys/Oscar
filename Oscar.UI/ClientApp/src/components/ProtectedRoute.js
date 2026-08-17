import { useNavigate } from 'react-router-dom';
import { useIsAuthenticated } from "@azure/msal-react";
import { useEffect } from 'react';

export default ({ children }) => {
  const isAuthenticated = useIsAuthenticated();
  let navigate = useNavigate();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/');
    }
  }, []);


  return children;
};