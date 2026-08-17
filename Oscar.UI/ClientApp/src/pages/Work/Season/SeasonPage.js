import React, { useState } from 'react';
import SlidingTable from '../../../shared/components/SlidingTable';
import seasonColumns from './seasonColumns';
import SeasonDetails from './SeasonDetails';
import Filters from '../Filters';

export default () =>  {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({searchObjects: [{searchEntity: "Works", searchColumn: "discriminator", searchText: "season"}]});
    const [tempFilters, setTempFilters] = useState(filters);
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const search = () => {
        setFilters(tempFilters);
    }

    const editSeason = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <Filters title='Season table filters' search={search} tempFilters={tempFilters} setTempFilters={setTempFilters}/>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'works/get'}
                    title={"Season"}
                    columns={seasonColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    onEdit={editSeason}
                >
                </SlidingTable>
            </div>
            <SeasonDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}