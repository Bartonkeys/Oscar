import React, { useState } from "react";
import { makeStyles } from '@mui/styles';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { alpha } from '@mui/material/styles';
import { AppBar, Toolbar, IconButton, MenuItem, Menu, Drawer }  from "@mui/material";
import { DensityMedium, AccountCircle, ExpandMore } from "@mui/icons-material";
import { canEdit, getClientName, isAdmin, logout } from "../helpers/client";
// import history from '../helpers/history';
import { useLocation, useNavigate } from "react-router-dom";
// import { Authentication } from "../../components/Authentication";
import { useMsal, useIsAuthenticated } from "@azure/msal-react";
import { loginRequest } from "../../authConfig";

const theme = createTheme();
const useStyles = makeStyles((theme) => ({
  grow: {
    flexGrow: 1,
  },
  menuButton: {
    marginRight: 2,
  },
  title: {
    display: "none",
    [theme.breakpoints.up("sm")]: {
      display: "block",
    },
  },
  search: {
    position: "relative",
    borderRadius: theme.shape.borderRadius,
    backgroundColor: alpha(theme.palette.common.white, 0.15),
    "&:hover": {
      backgroundColor: alpha(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing(2),
    marginLeft: 0,
    width: "100%",
    [theme.breakpoints.up("sm")]: {
      marginLeft: theme.spacing(3),
      width: "auto",
    },
  },
  searchIcon: {
    padding: theme.spacing(0, 2),
    height: "100%",
    position: "absolute",
    pointerEvents: "none",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
  },
  inputRoot: {
    color: "inherit",
  },
  inputInput: {
    padding: theme.spacing(1, 1, 1, 0),
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)}px)`,
    transition: theme.transitions.create("width"),
    width: "100%",
    [theme.breakpoints.up("md")]: {
      width: "20ch",
    },
  },
  sectionDesktop: {
    display: "none",
    [theme.breakpoints.up("md")]: {
      display: "flex",
    },
  },
  sectionMobile: {
    display: "flex",
    [theme.breakpoints.up("md")]: {
      display: "none",
    },
  },
}));

function PrimarySearchAppBar(props) {
  const classes = useStyles();
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [mobileMoreAnchorEl, setMobileMoreAnchorEl] = React.useState(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const location = useLocation();
  let navigate = useNavigate();

  const isMenuOpen = Boolean(anchorEl);
  const isMobileMenuOpen = Boolean(mobileMoreAnchorEl);
  const { instance } = useMsal();

  const handleProfileMenuOpen = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMobileMenuClose = () => {
    setMobileMoreAnchorEl(null);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    handleMobileMenuClose();
  };

  const handleMobileMenuOpen = (event) => {
    setMobileMoreAnchorEl(event.currentTarget);
  };

  // function handleLogout() {
  //   logout();
  // }

  const handleLogout = () => {
    instance.logoutPopup({
      postLogoutRedirectUri: "/",
      mainWindowRedirectUri: "/"
  });
  handleMenuClose();
}

  const handleLogin = () => {
    instance.loginPopup(loginRequest).catch(e => {
      console.log(e);
    });
    handleMenuClose();
  }

  function handleMyAccount() {
    navigate('/account');
    handleMenuClose();
  }

  function toggleDrawer() {
    setIsDrawerOpen(!isDrawerOpen);
  }

  function goToRoute(route) {
    if (route !== location.pathname) {
      navigate(route);
      setIsDrawerOpen(false);
    }
  }


  const menuId = "primary-search-account-menu";
  const isAuthenticated = useIsAuthenticated();
  const renderMenu = (
    <Menu
      anchorEl={anchorEl}
      anchorOrigin={{ vertical: "top", horizontal: "right" }}
      id={menuId}
      keepMounted
      transformOrigin={{ vertical: "top", horizontal: "right" }}
      open={isMenuOpen}
      onClose={handleMenuClose}
    >
      { isAuthenticated ?
      <MenuItem onClick={handleLogout}>Logout</MenuItem> :
      <MenuItem onClick={handleLogin}>Login</MenuItem> }
    </Menu>
  );

  const mobileMenuId = "primary-search-account-menu-mobile";
  const renderMobileMenu = (
    <Menu
      anchorEl={mobileMoreAnchorEl}
      anchorOrigin={{ vertical: "top", horizontal: "right" }}
      id={mobileMenuId}
      keepMounted
      transformOrigin={{ vertical: "top", horizontal: "right" }}
      open={isMobileMenuOpen}
      onClose={handleMobileMenuClose}
    >
      { isAuthenticated ?
      <MenuItem onClick={handleLogout}>Logout</MenuItem> :
      <MenuItem onClick={handleLogin}>Login</MenuItem> }
    </Menu>
  );

  return (
    <div className={classes.grow}>
      <AppBar position="static">
        <Toolbar>
        { isAuthenticated &&
          <IconButton
            edge="start"
            className={classes.menuButton}
            color="inherit"
            onClick={toggleDrawer}
          >
          <DensityMedium />
          </IconButton>
        }

          <div className={classes.grow} />
          <div className={classes.sectionDesktop}>
            <div className="mr-3">
              {/* <h2 className="dark">Client name here</h2> */}
            </div>
            <IconButton
              edge="end"
              aria-label="account of current user"
              aria-controls={menuId}
              aria-haspopup="true"
              onClick={handleProfileMenuOpen}
              color="inherit"
            >
              <AccountCircle />
            </IconButton>
          </div>
          <div className={classes.sectionMobile}>
            <IconButton
              aria-label="show more"
              aria-controls={mobileMenuId}
              aria-haspopup="true"
              onClick={handleMobileMenuOpen}
              color="inherit"
            >
            <ExpandMore />
            </IconButton>
          </div>
        </Toolbar>
      </AppBar>
      {renderMobileMenu}
      {renderMenu}

      { isAuthenticated &&
        <Drawer open={isDrawerOpen} onClose={toggleDrawer}>
          <div className="flexCol p-3" style={{ width: '300px' }}><div>
              <h3>Admin</h3>
              <MenuItem selected={location.pathname === '/allclients'} onClick={() => goToRoute('/allclients')}><div className="linkText">Clients</div></MenuItem>
              <MenuItem selected={location.pathname === '/allmatches'} onClick={() => goToRoute('/allmatches')}><div className="linkText">Matches</div></MenuItem>
              <h4>Works</h4>
              <MenuItem selected={location.pathname === '/allworks'} onClick={() => goToRoute('/allworks')}><div className="linkText">Works</div></MenuItem>
              <MenuItem selected={location.pathname === '/allseries'} onClick={() => goToRoute('/allseries')}><div className="linkText">Series</div></MenuItem>
              <MenuItem selected={location.pathname === '/allseasons'} onClick={() => goToRoute('/allseasons')}><div className="linkText">Seasons</div></MenuItem>
              <MenuItem selected={location.pathname === '/allepisodes'} onClick={() => goToRoute('/allepisodes')}><div className="linkText">Episodes</div></MenuItem>
              <MenuItem selected={location.pathname === '/allstandalone'} onClick={() => goToRoute('/allstandalone')}><div className="linkText">Standalone</div></MenuItem>
            </div> 
          </div>
        </Drawer>
      }
    </div>
  );
}
export default PrimarySearchAppBar;
