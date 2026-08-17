import Autocomplete from '@mui/lab/Autocomplete';
import TextField from '@mui/material/TextField';
import CircularProgress from '@mui/material/CircularProgress';
import { getClientType } from '../helpers/client';
import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { distinct, DistinctBy, sleep } from '../helpers/jshelper';

let search = 0;

export default function AsyncComplete(props) {
    const [open, setOpen] = useState(false);
    const [options, setOptions] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [loading, setLoading] = useState(false);
    const [selectedOptions, setSelectedOptions] = useState([]);

    async function lazySearch(term) {
        if (!props.emptySearch && !term) {
            return;
        }

        search++;
        if (!loading) {
            setLoading(true);
        }
        await sleep(350);
        if (search === 1) {
            if (term === searchTerm) {
                setLoading(false);
            }
            setSearchTerm(term);
        }
        search--;
    }

    useEffect(() => {
        let active = true;

        if (!props.emptySearch && !searchTerm) {
            setLoading(false);
            setOptions([]);
            return undefined;
        }

        (async () => {
            try {
                let client = getClientType(props.client);
                const response = await client.get(props.searchUri + searchTerm, {
                    params: props.otherParams
                });
                if (active) {
                    setOptions(response.data);
                }
            }
            catch { }
            setLoading(false);
        })();

        return () => {
            active = false;
            setLoading(false);
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [searchTerm, props.searchUri, props.otherParams, props.emptySearch]);

    function blurFunc(value) {

        if (!props.multiple && !props.returnFunc) {
            props.getValue(value.target.value, selectedOptions.length > 0 && selectedOptions[0]?.toLowerCase() === value.target.value.toLowerCase());
        }
    }

    function changeSelection(e, value, reason) {
        let returnValue = '';

        if (reason === 'clear') {
            lazySearch('');
        }

        if (props.multiple) {
            let newOptions = [...selectedOptions];
            if (reason === 'select-option') {
                let distinctOptions = !props.displayFunc ? distinct(options.concat(selectedOptions)) : DistinctBy(options.concat(selectedOptions), props.displayFunc);
                newOptions = distinctOptions.filter(x => value.includes(props.displayFunc ? props.displayFunc(x) : x));
                setSelectedOptions(newOptions);
                lazySearch('');
            }
            else if (reason === 'remove-option' || reason === 'clear') {
                newOptions = selectedOptions.filter(x => value.includes(props.displayFunc ? props.displayFunc(x) : x));
                setSelectedOptions(newOptions);
            }

            returnValue = [];
            if (newOptions.length > 0) {
                returnValue = [...newOptions];
            }
            if (props.returnFunc) {
                returnValue = returnValue.map(x => props.returnFunc(x));
            }
        }
        else {

            let option = options.find(x => props.displayFunc ? props.displayFunc(x) === value : x === value);
            let newOptions = [option];
            setSelectedOptions(newOptions);

            returnValue = newOptions.length > 0 ? newOptions[0] : '';
            if (props.returnFunc && newOptions.length > 0) {
                returnValue = props.returnFunc(returnValue);
            }
        }

        props.getValue(returnValue, true);
    }

    function getOptions() {
        let returnOptions = [];
        if (props.multiple) {
            returnOptions = !props.displayFunc ? distinct(options.concat(selectedOptions)) : DistinctBy(options.concat(selectedOptions), props.displayFunc);
        }
        else {
            returnOptions = options;
        }

        if (props.displayFunc) {
            returnOptions = returnOptions.map(x => props.displayFunc(x));
        }

        return returnOptions;
    }

    return (<Autocomplete
        open={open}
        onOpen={() => setOpen(true)}
        onClose={() => setOpen(false)}
        getOptionLabel={(option) => option}
        filterOptions={(options, state) => options}
        options={getOptions()}
        loading={loading}
        freeSolo={Boolean(!props.returnFunc) || Boolean(!props.multiple)}
        size='small'
        multiple={props.multiple}
        onChange={changeSelection}
        renderInput={(params) => (
            <TextField
                {...params}
                onKeyUpCapture={(value) => lazySearch(value.target.value)}
                label={props.label}
                variant="standard"
                placeholder="Type to display options"
                onBlur={blurFunc}
                InputProps={{
                    ...params.InputProps,
                    endAdornment: (
                        <React.Fragment>
                            {loading ? <CircularProgress color="inherit" size={20} /> : null}
                            {params.InputProps.endAdornment}
                        </React.Fragment>
                    ),
                }}
            />
        )}
    ></Autocomplete>);
}

AsyncComplete.propTypes = {
    searchUri: PropTypes.string.isRequired,
    getValue: PropTypes.func.isRequired,
    label: PropTypes.string.isRequired,
    displayFunc: PropTypes.func,
    returnFunc: PropTypes.func,
    multiple: PropTypes.bool,
    emptySearch: PropTypes.bool,
    otherParams: PropTypes.object,
    client: PropTypes.string.isRequired
}