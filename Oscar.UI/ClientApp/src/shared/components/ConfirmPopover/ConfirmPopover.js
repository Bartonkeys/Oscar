import { Backdrop, Button, CircularProgress, Fade, IconButton, Modal } from '@mui/material';
import { Close } from '@mui/icons-material';
import PropTypes from 'prop-types';
import React, { useEffect, useState } from 'react';
import './confirmPopover.css';

export default function ConfirmPopover(props) {
    const [open, setOpen] = useState(true);
    const [actionExecuting, setActionExecuting] = useState(false);

    useEffect(() => {
        setOpen(Boolean(props.open));
    }, [props.open]);

    useEffect(() => {
        if (!open) {
            props.closeModel();
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [open]);

    return (
        <Modal className="flexCol"
            onClick={(e) => { e.stopPropagation(); }}
            onClose={() => setOpen(false)}
            open={open}

            closeAfterTransition
            BackdropComponent={Backdrop}
            BackdropProps={{
                timeout: 300,
            }}
        >
            <Fade in={open} >
                <div className="modalArea">
                    <div className="flexRow flexApart">
                        <h2 >{props.title}</h2>
                        <div>
                            <IconButton onClick={() => setOpen(false)}><Close /></IconButton>
                        </div>
                    </div>
                    <div className="mb-3 minorText">{props.question}</div>
                    <Button
                        size="large"
                        variant="contained"
                        color={props.color || "secondary"}
                        onClick={async () => { setActionExecuting(true); setOpen(Boolean(await props.action())); setActionExecuting(false); }}
                        disabled={actionExecuting}
                    >{actionExecuting ? <CircularProgress size={26} /> : 'Confirm'}</Button>
                </div>
            </Fade>
        </Modal>
    );
}

ConfirmPopover.propTypes = {
    open: PropTypes.bool.isRequired,
    question: PropTypes.string.isRequired,
    action: PropTypes.func.isRequired,
    closeModel: PropTypes.func.isRequired,
    title: PropTypes.string.isRequired,
    color: PropTypes.string
}