import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { InputLabel, Autocomplete, TextField } from '@mui/material';
import { CircularProgress } from '@mui/material';
import { get } from "../../helpers/apiaccess"

export const AutoComplete = (props) => {

    const { label, value, uri, onChange, keyField, nameField, nullValue, multiple } = props;

    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [error, setError] = useState(false);
    const [options, setOptions] = useState([]);

    useEffect(() => {
        (async () => {
            try {
                setFetching(true);
                setFetched(false);
                let options = await get(uri);
                if(options.hasOwnProperty('records')){
                    options = options.records;
                }
                setOptions(options);
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
            <Autocomplete
                multiple={multiple}
                id="tags-outlined"
                options={options}
                getOptionLabel={(option) => option[nameField]}
                // defaultValue={[top100Films[13]]}
                filterSelectedOptions
                onChange={onChange}
                renderInput={(params) => (
                <TextField
                    {...params}
                    label={label}
                />
                )}
            />
        )}
        </>
    );
}

// EnumList.propTypes = {
//     label: PropTypes.string,
//     value: PropTypes.any,
//     uri: PropTypes.string
// };
