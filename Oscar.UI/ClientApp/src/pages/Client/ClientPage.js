import { Button, TextField } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../shared/components/SlidingTable';
import { AUTH } from '../../shared/helpers/client';
import { sleep } from '../../shared/helpers/jshelper';
import clientColumns from './clientColumns';
import ClientDetails from './ClientDetails';

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

    const createClient = () => {
        setId(0);
        setOpenEdit(true);    
    }

    const editClient = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <div className="raisedContainer">
                <div className="flexRow">
                    <h2>Client Table Filters</h2>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            label="Reference"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'clientReference')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Name"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'clientName')}
                        ></TextField>
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Email"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'email')}
                        ></TextField>
                    </div>
                </div>
            </div>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'client/get'}
                    title={"Clients"}
                    columns={clientColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    client={AUTH}
                    onEdit={editClient}>
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={createClient}>Create Client</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <ClientDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}