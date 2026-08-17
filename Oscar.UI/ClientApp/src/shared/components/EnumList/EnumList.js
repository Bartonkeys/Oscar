import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { InputLabel, MenuItem, Select } from '@mui/material';
import { CircularProgress } from '@mui/material';
import { get } from "../../helpers/apiaccess"

export const EnumList = (props) => {

    const { label, value, uri, onChange, keyField, nameField, nullValue } = props;

    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [error, setError] = useState(false);
    const [enums, setEnums] = useState([]);

    useEffect(() => {
        (async () => {
            try {
                setFetching(true);
                setFetched(false);
                let enums = await get(uri);
                if(enums.hasOwnProperty('records')){
                    enums = enums.records;
                }
                setEnums(enums);
                setFetching(false);
                setFetched(true);
            }
            catch { 
                setError(true);
                setFetching(false);
            }
        })();
    }, []);

    return (
        <>
        { fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
        { error && <div>Error loading list</div> }
        { !fetching && fetched && (
            <>
            <InputLabel>{label}</InputLabel>
            <Select
                value={value}
                label={label}
                onChange={onChange}
            >
            <MenuItem value={nullValue} key={label}>Please select...</MenuItem>
            {enums.map((item, index) => (<MenuItem value={item[keyField]} key={index}>{item[nameField]}</MenuItem>))}
            </Select>
            </>
        )}
        </>
    );
}

EnumList.propTypes = {
    label: PropTypes.string,
    value: PropTypes.any,
    uri: PropTypes.string
};
