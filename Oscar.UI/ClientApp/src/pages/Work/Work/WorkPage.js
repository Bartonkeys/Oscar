import { Button, TextField, MenuItem, Select, FormControl, InputLabel } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../../shared/components/SlidingTable';
import workColumns from './workColumns';
import WorkDetails from './WorkDetails';
import Filters from '../Filters';

export default () =>  {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({});
    const [tempFilters, setTempFilters] = useState(filters);
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);
    const [discriminator, setDiscriminator] = useState('Series');
    const [filterDiscriminator, setFilterDiscriminator] = useState('All');

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const setFilter = async(e, searchQueryParam) => {
        let value = e.target.value;
        setTempFilters({ ...tempFilters, [searchQueryParam]: value});
        if(searchQueryParam === 'Discriminator'){
            setFilterDiscriminator(value);
        }
    }

    const search = () => {
        setFilters(tempFilters);
    }

    const clearAll = () => {
        setTempFilters({});
    }

    const handleSubmit = event => {
        //prevent page refresh
        event.preventDefault();
    };

    const createWork = () => {
        setId(0);
        setDiscriminator("Series");
        setOpenEdit(true);    
    }

    const editWork = ({id, discriminator}) => {
        setId(id);
        setDiscriminator(discriminator);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
        {/* <div className="raisedContainer">
                <div className="flexRow">
                    <h2>Works Table Filters</h2>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="Title"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'title')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={search}>Search</Button>
                    </div>
                </div>
        </div> */}
        <form onSubmit={handleSubmit}>
            <div className="raisedContainer">
                <div className="flexRow">
                    <h2>Works Table Filters</h2>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            label="Title"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'Title')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Actor first name"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ActorFirstName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Actor surname"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ActorLastName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Director first name"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'DirectorFirstName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Director surname"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'DirectorLastName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <Button
                            variant="contained"
                            color="primary"
                            type="reset"
                            onClick={clearAll}>Clear all</Button>
                    </div>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            label="Producer first name"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ProducerFirstName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Producer surname"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ProducerLastName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Screenwriter first name"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ScreenWriterFirstName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Screenwriter surname"
                            size="small"
                            variant="standard"
                            type="search"
                            onInput={(e) => setFilter(e, 'ScreenWriterLastName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <FormControl size="small">
                            <InputLabel>Type</InputLabel>
                            <Select
                                value={filterDiscriminator}
                                label='Type'
                                onChange={(e) => setFilter(e, 'Discriminator')}
                            >
                            <MenuItem value="All" key="All">All</MenuItem>
                            <MenuItem value="Series" key="Series">Series</MenuItem>
                            <MenuItem value="Season" key="Season">Season</MenuItem>
                            <MenuItem value="Episode" key="Episode">Episode</MenuItem>
                            <MenuItem value="StandAlone" key="StandAlone">StandAlone</MenuItem>
                            </Select>
                        </FormControl>
                    </div>
                    <div className="inputItem">
                        <Button
                            variant="contained"
                            color="primary"
                            type="submit"
                            onClick={search}>Search</Button>
                    </div>
                </div>
            </div>
        </form>
        <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchByQueryString={true}
                    searchUri={'works/title/search'}
                    title={"Works"}
                    columns={workColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    onEditWholeRow={editWork}
                >
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={createWork}>Create Series</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <WorkDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id} discriminator={discriminator}/>
        </div>
    );
}