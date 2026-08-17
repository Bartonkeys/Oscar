import React, { useState } from 'react';
import SlidingTable from '../../../shared/components/SlidingTable';
import episodeColumns from './episodeColumns';
import EpisodeDetails from './EpisodeDetails';
import Filters from '../Filters';

export default () =>  {
    const [takeValue, setTakeValue] = useState(20);
    const [filters, setFilters] = useState({searchObjects: [{searchEntity: "Works", searchColumn: "discriminator", searchText: "episode"}]});
    const [tempFilters, setTempFilters] = useState(filters);
    const [openEdit, setOpenEdit] = useState(false);
    const [id, setId] = useState(0);

    const refreshList = () =>{
        setFilters(g => { return { ...g } });
    }

    const search = () => {
        setFilters(tempFilters);
    }

    const editEpisode = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="AppBody">
            <Filters title='Episode table filters' search={search} tempFilters={tempFilters} setTempFilters={setTempFilters}/>
            <div className="anchor flexGrow">
                <SlidingTable
                    left={0}
                    searchFilters={filters}
                    searchUri={'works/get'}
                    title={"Episode"}
                    columns={episodeColumns}
                    selectRow={() => { }}
                    takeValue={takeValue}
                    setTakeValue={(take) => setTakeValue(take)}
                    static={true}
                    pageNumber={filters.pageNumber}
                    onEdit={editEpisode}
                >
                </SlidingTable>
            </div>
            <EpisodeDetails open={openEdit} toggleDrawer={setOpenEdit} refreshList={refreshList} id={id}/>
        </div>
    );
}