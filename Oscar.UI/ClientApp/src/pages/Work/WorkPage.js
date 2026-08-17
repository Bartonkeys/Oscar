import { Button, TextField } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../shared/components/SlidingTable';
import history from '../../shared/helpers/history';
import { sleep } from '../../shared/helpers/jshelper';
import workColumns from './workColumns';
import Details from './Details';

let typeName = 0;

export default () =>  {
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

    function createWork() {
        setId(0);
        setOpenEdit(true);    
    }

    return (
        <div className="AppBody">
            <div className="raisedContainer">
                <div className="flexRow">
                    <h2>Works Table Filters</h2>
                </div>
                <div className="flexRow">
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="Reference"
                            size="small"
                            variant="standard"
                            onInput={(e) => setFilter(e, 'reference')}
                        ></TextField>
                    </div>
                </div>
            </div>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'works/get'}
                    title={"Works"}
                    columns={workColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                >
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={createWork}>Create Work</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <Details open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}