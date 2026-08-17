import { TextField, Button } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../../shared/components/SlidingTable';
import standaloneColumns from './standaloneColumns';
import StandaloneDetails from './StandaloneDetails';
import Filters from '../Filters';

export default () =>  {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({searchObjects: [{searchEntity: "Works", searchColumn: "discriminator", searchText: "standalone"}]});
    const [tempFilters, setTempFilters] = useState(filters);
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const search = () => {
        setFilters(tempFilters);
    }

    const createStandalone = () => {
        setId(0);
        setOpenEdit(true);    
    }

    const editStandalone = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <Filters title='Standalone table filters' search={search} tempFilters={tempFilters} setTempFilters={setTempFilters}/>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'works/get'}
                    title={"Standalone"}
                    columns={standaloneColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    onEdit={editStandalone}
                >
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={createStandalone}>Create Standalone</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <StandaloneDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}