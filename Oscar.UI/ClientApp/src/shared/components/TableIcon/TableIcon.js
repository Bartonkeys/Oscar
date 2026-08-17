import { CircularProgress, IconButton } from '@mui/material';
import React, { useState } from 'react';
import PropTypes from 'prop-types';

export default function TableIcon(props) {
    const [isActing, setIsActing] = useState(false);

    function clickEvent(e) {
        if (!isActing) {
            setIsActing(true);

            (async () => {
                await props.clickAction(e);
                setIsActing(false);
            })();
        }
    }

    return (
        <div><IconButton title={isActing ? '' : props.title} size="small" disabled={isActing} onClick={clickEvent} >{isActing ? <CircularProgress size={props.spinnerSize || 24} /> : props.children}</IconButton></div>
    )
}

TableIcon.propTypes = {
    clickAction: PropTypes.func.isRequired,
    title: PropTypes.string,
    spinnerSize: PropTypes.number
}

