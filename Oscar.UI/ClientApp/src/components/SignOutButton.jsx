import React from "react";
import { useMsal } from "@azure/msal-react";
import DropdownButton from "react-bootstrap/DropdownButton";
import Dropdown from "react-bootstrap/esm/Dropdown";
import AccountCircleIcon from '@mui/icons-material/AccountCircle';

/**
 * Renders a sign-out button
 */
export const SignOutButton = () => {
    const { instance } = useMsal();

    const handleLogout = (logoutType) => {
        if (logoutType === "popup") {
            instance.logoutPopup({
                postLogoutRedirectUri: "/",
                mainWindowRedirectUri: "/"
            });
        } else if (logoutType === "redirect") {
            instance.logoutRedirect({
                postLogoutRedirectUri: "/",
            });
        }
    }
    return (
        <DropdownButton drop="left" title={
            <AccountCircleIcon />
        }>
            <Dropdown.Item as="button" onClick={() => handleLogout("popup")}>Sign out using Popup</Dropdown.Item>
            <Dropdown.Item as="button" onClick={() => handleLogout("redirect")}>Sign out using Redirect</Dropdown.Item>
        </DropdownButton>
    )
}