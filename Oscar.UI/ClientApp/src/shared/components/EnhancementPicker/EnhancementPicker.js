import React from 'react';
import PropTypes from 'prop-types';
import { Badge, FormControl, InputLabel, MenuItem, Select } from '@mui/material';

export default function EnhancementPicker(props) {

    return (
        <FormControl title="Enhanced records will not be overwitten with new transmission details" style={{ width: '200px' }} size="small">
            <InputLabel shrink>Enhancement</InputLabel>
            <Select displayEmpty={true} label="Enhancement" value={props.value || ''} onChange={props.onChange}>
                <MenuItem value=""><div className="flexRow flexCentre ml-2"><Badge variant="dot" badgeContent="" color="secondary" /><div className="ml-3">None</div></div></MenuItem>
                <MenuItem value="ManualEnhanced"><div className="flexRow flexCentre ml-2"><Badge variant="dot" badgeContent="" color="primary" /><div className="ml-3">Manually Enhanced</div></div></MenuItem>
                <MenuItem disabled={true} value="AutoEnhanced"><div className="flexRow flexCentre ml-2"><Badge variant="dot" badgeContent="" color="primary" /><div className="ml-3">Auto Enhanced</div></div></MenuItem>
                <MenuItem disabled={true} value="QuickCreated"><div className="flexRow flexCentre ml-2"><Badge variant="dot" badgeContent="" color="secondary" /><div className="ml-3">Quick Created</div></div></MenuItem>
            </Select>
        </FormControl>
    );
}

EnhancementPicker.propTypes = {
    onChange: PropTypes.func.isRequired,
    value: PropTypes.string
};