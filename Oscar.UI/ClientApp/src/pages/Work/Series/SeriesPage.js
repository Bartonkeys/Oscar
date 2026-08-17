import { Button, TextField } from '@mui/material';
import React, { useState } from 'react';
import SlidingTable from '../../../shared/components/SlidingTable';
import seasonColumns from './seriesColumns';
import SeriesDetails from './SeriesDetails';
import Filters from '../Filters';

export default () =>  {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({searchObjects: [{searchEntity: "Works", searchColumn: "discriminator", searchText: "series"}]});
    const [tempFilters, setTempFilters] = useState(filters);
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const search = () => {
        setFilters(tempFilters);
    }

    const createSeries = () => {
        setId(0);
        setOpenEdit(true);    
    }

    const editSeries = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <Filters title='Series table filters' search={search} tempFilters={tempFilters} setTempFilters={setTempFilters}/>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'works/get'}
                    title={"Series"}
                    columns={seasonColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    onEdit={editSeries}
                >
                    <div className="flexRow">
                        <div className="flexRight">
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={createSeries}>Create Series</Button>
                        </div>
                    </div>
                </SlidingTable>
            </div>
            <SeriesDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}