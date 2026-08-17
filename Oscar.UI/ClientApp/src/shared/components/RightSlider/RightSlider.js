import { Button, CircularProgress, Drawer, IconButton } from '@mui/material';
import { Close } from '@mui/icons-material';
import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import './rightSlider.css';

export default function RightSlider(props) {
    const [drawerState, setDrawState] = useState({
        saving: false,
        open: false,
        initial: true
    });

    function closeDrawer() {
        props.toggleDrawer(false);
    }

    function toggleOpen() {
        setDrawState({ ...drawerState, open: !drawerState.open });
    }

    function saveAction() {
        setDrawState({ ...drawerState, saving: true });

        (async () => {
            try {
                await props.onSave()
            } catch { }
            setDrawState({ ...drawerState, saving: false });
        })();
    }

    return (
        <React.Fragment key="right">
            <Drawer
                anchor="right"
                open={props.open}
                onClose={() => props.toggleDrawer(false)}
            >
                <div className={drawerState.open ? "sliderDrawer" : "sliderDrawer"}>
                    <div className="flexRow m-3">
                        <div>
                            <h2>{props.title}</h2>
                        </div>

                        <div className="moveRight">
                            <IconButton onClick={closeDrawer} ><Close /></IconButton>
                        </div>
                    </div>
                    <div className="flexCol p-3 flexGrow scrollVertical">
                        {props.children}
                    </div>
                    {props.canSave ?
                        <div className="sliderSave">
                            <Button
                                size="large"
                                variant="contained"
                                color="primary"
                                onClick={saveAction}
                                disabled={drawerState.saving}
                                fullWidth={true}
                            >{drawerState.saving ? <CircularProgress size={26} /> : "Save Changes"}</Button>
                        </div>
                        : <div></div>}
                </div>
            </Drawer>
        </React.Fragment>
    );
}

RightSlider.propTypes = {
    onClose: PropTypes.func,
    title: PropTypes.string.isRequired,
    canSave: PropTypes.bool,
    onSave: PropTypes.func
}