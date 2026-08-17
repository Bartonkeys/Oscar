import React from "react";

import { useIsAuthenticated } from "@azure/msal-react";
import { SignInButton } from "./SignInButton";
import { SignOutButton } from "./SignOutButton";

/**
 * Renders the navbar component with a sign-in or sign-out button depending on whether or not a user is authenticated
 * @param props 
 */
export const Authentication = (props) => {
    const isAuthenticated = useIsAuthenticated();

    return (
        <>
                { isAuthenticated ? <SignOutButton /> : <SignInButton /> }
        </>
    );
};
