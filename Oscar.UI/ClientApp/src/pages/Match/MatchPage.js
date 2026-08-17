import { Button, TextField } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../shared/components/SlidingTable';
import { AUTH } from '../../shared/helpers/client';
import { sleep } from '../../shared/helpers/jshelper';
import columns from './columns';
import Details from './Details';

let typeName = 0;

export default () => {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({searchObjects: []});
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const setFilter = async(e, searchColumn) => {
        let value = e.target.value;
        typeName++;
        await sleep(350);
        if (typeName === 1) {
            let searchObjects = filters.searchObjects.filter((item) => item.searchColumn !== searchColumn);
            searchObjects.push({searchColumn, searchText: value});
            setFilters({ ...filters, pageNumber: 1, searchObjects });
        }

        typeName--;
    }

    const create = () => {
        setId(0);
        setOpenEdit(true);    
    }

    const edit = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <div className="raisedContainer">
                <div className="flexRow">
                    <h2>Match Filters</h2>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            label="Reference"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'reference')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Requested by"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'requestedBy')}
                        ></TextField>
                    </div>
                </div>
            </div>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'matchRequest/get'}
                    title={"Matches"}
                    columns={columns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    client={AUTH}>
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={create}>Create Match</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <Details open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}