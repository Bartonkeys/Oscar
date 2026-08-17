import { useMsal, useIsAuthenticated } from "@azure/msal-react";
import { loginRequest } from "../../authConfig";
import Link from '@mui/material/Link';

export default () => {
    const isAuthenticated = useIsAuthenticated();
    const { instance } = useMsal();

    const handleLogin = () => {
        instance.loginPopup(loginRequest).catch(e => {
          console.log(e);
        });
      }

    return (
        <div className="AppBody">
            <div className="raisedContainer" style={{paddingTop: 50, paddingBottom: 50}}>
                <h1>Welcome to Oscar UI</h1>
                { isAuthenticated ?
                    <span>Please select a menu option</span> :
                    <span>Please <Link href="#" onClick={handleLogin}>login</Link> to access the menu options</span> 
                }
            </div>
        </div>
    );
}