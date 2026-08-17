import { TextField, Button } from '@mui/material';
import React from 'react';

const Filters = ({title, search, tempFilters, setTempFilters}) => {
    const setFilter = async(e, searchEntity, searchColumn) => {
        let value = e.target.value;
        let searchObjects = tempFilters.searchObjects.filter((item) =>
            item.searchEntity !== searchEntity ||
            item.searchColumn !== searchColumn);
        searchObjects.push({searchEntity, searchColumn, searchText: value});
        setTempFilters({ ...tempFilters, pageNumber: 1, searchObjects });
    }
    return (
        <div className="raisedContainer">
            <div className="flexRow">
                <h2>{title}</h2>
            </div>
            <div className="flexRow">
                <div className="inputItem">
                    <TextField
                        label="Title"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Titles', 'title')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Actor first name"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Actors', 'FirstName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Actor surname"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Actors', 'LastName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Director first name"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Directors', 'FirstName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Director surname"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Directors', 'LastName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                </div>
            </div>
            <div className="flexRow">
                <div className="inputItem">
                    <TextField
                        label="Producer first name"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Producers', 'FirstName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Producer surname"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'Producers', 'LastName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Screenwriter first name"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'ScreenWriters', 'FirstName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Screenwriter surname"
                        size="small"
                        variant="standard"
                        onInput={(e) => setFilter(e, 'ScreenWriters', 'LastName')}
                    ></TextField>
                </div>
                <div className="inputItem">
                </div>
                <div className="inputItem">
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={search}>Search</Button>
                </div>
            </div>
        </div>
    );
}

export default Filters;
