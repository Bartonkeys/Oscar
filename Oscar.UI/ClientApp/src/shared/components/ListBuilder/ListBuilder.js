import { Badge, Button, IconButton, TextField } from '@mui/material';
import { Close } from '@mui/icons-material';
import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import './listBuilder.css';
import AsyncComplete from '../AsyncComplete';

export default function ListBuilder(props) {
    const [listItems, setListItems] = useState([]);
    const [newItem, setNewItem] = useState('');

    useEffect(() => {
        if (props.list) {
            setListItems([...props.list]);
        }
    }, [props.list]);

    function removeItem(value) {
        let newListItems = listItems.filter(x => x[props.displayField] !== value);
        setListItems(newListItems);
        props.onChange([...newListItems]);
    }

    function addItem() {
        if (props.fetchUri) {
            addItemAsync();
            return;
        }

        if (newItem) {
            let existingValues = listItems.map(x => x[props.displayField]?.toString()?.toLowerCase());
            if (!existingValues.includes(newItem?.toLowerCase())) {
                let newList = [...listItems];
                let newListItem = {};
                newListItem[props.displayField] = newItem;
                newList.push(newListItem);
                setListItems(newList);
                props.onChange([...newList]);
            }

            setNewItem('');
        }
    }

    function addItemAsync() {
        if (newItem) {
            let existingValues = listItems.map(x => x[props.displayField]);
            if (!existingValues.includes(newItem[props.displayField])) {
                let newList = [...listItems];
                newList.push(newItem);
                setListItems(newList);
                props.onChange([...newList]);
            }
        }
    }

    function updateNewItem(e) {
        setNewItem(e.target.value);
    }

    function updateNewItemAsync(item) {
        setNewItem({ ...item });
    }

    let searchBox = props.fetchUri ? <AsyncComplete label={"New " + props.title} searchUri={props.fetchUri} displayFunc={props.displayFunc} returnFunc={props.returnFunc} getValue={updateNewItemAsync} client={props.client} /> :
        <TextField fullWidth={true} label={"New " + props.title} size="small" variant="standard" value={newItem} onChange={updateNewItem} />;

    return (
        <div className="raisedContainerSmall">
            <div className="builderTitle">
                <h3>{props.title}</h3>
                <div className="mr-3"><Badge
                    badgeContent={listItems.length}
                    color="primary"
                ></Badge>
                </div>
            </div>
            <div className="flexCol p-2">
                {!props.disabled ?
                    <div className="flexRow">
                        <div className="inputItem">
                            {searchBox}
                        </div>
                        <div className="flexCol flexMiddle ml-2 mr-1">
                            <Button size="small" variant="contained" color="primary" onClick={addItem} >Add</Button>
                        </div>
                    </div>
                    : <div></div>}
                <div className="builderListList flexCol">
                    {listItems.map((x, i) =>
                        <div key={x[props.displayField] || (props.displayFunc ? props.displayFunc(x) : i)} className="builderListItem flexCol">
                            <div className="flexRow flexCentre flexApart">
                                <div>{props.displayFunc ? props.displayFunc(x) : x[props.displayField]}</div>
                                <div className="flexRow flexCentre">
                                    {props.childFunc ? props.childFunc(x) : <div></div>}
                                    {!props.disabled ? <div><IconButton title="Remove" onClick={() => removeItem(x[props.displayField])} color="secondary" size="small"><Close /></IconButton></div> : <div></div>}
                                </div>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

ListBuilder.propTypes = {
    title: PropTypes.string.isRequired,
    displayField: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    client: PropTypes.string,
    current: PropTypes.array,
    disabled: PropTypes.bool,
    fetchUri: PropTypes.string,
    displayFunc: PropTypes.func,
    returnFunc: PropTypes.func,
    childFunc: PropTypes.func
};