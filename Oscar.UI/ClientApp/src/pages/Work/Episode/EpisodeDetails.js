import { CircularProgress} from '@mui/material';
import React, { useEffect, useState } from 'react';
import { toastr } from '../../../shared/helpers/toast';
import { get, create, update, showValidationError } from "../../../shared/helpers/apiaccess"
import BaseWork from '../BaseWork'
import defaultEpisode from '../baseWorkDefault'
import RightSlider from '../../../shared/components/RightSlider/RightSlider';

export default function EpisodeDetails({open, id, seasonId, seriesId, toggleDrawer, onSuccessfulSave}) {
    defaultEpisode.id = id;
    if(seasonId){
        defaultEpisode.seasonId = seasonId;
    }
    if(seriesId){
        defaultEpisode.seriesId = seriesId;
    }

    const [work, setWork] = useState(defaultEpisode);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        (async () => {
            try {
                setWork(defaultEpisode);
                if (id > 0) {
                    setFetching(true);
                    setFetched(false);
                    let retrievedWork = await get(`episode/${id}`);
                    retrievedWork.seasonId = seasonId;
                    setWork(retrievedWork);
                    setFetching(false);
                    setFetched(true);
                }
            }
            catch { 
                toastr('error', 'Error retrieving episode');
            }
        })();
    }, [open]);

    const onClose = () => {
        toggleDrawer();
        setFetching(false);
        setFetched(false);
    }

    const saveWork = async() => {
        try {

            setSaving(true);
            let response = work.id ?
            await update('episode/' + work.id, work) :
            await create('episode', work);

            const action = id === 0? 'created': 'updated';
            setSaving(false);
            toastr('success', `Episode ${action} successfully`);
            onSuccessfulSave();
            onClose();
        }
        catch (err) {
            setSaving(false);
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error updating episode`);
            }
        }
    }

    return (
        <RightSlider canSave={true} title={id === 0? "Create Episode": "Episode Details"} onSave={saveWork}
        onClose={onClose} open={open} toggleDrawer={toggleDrawer} >
            <div className="flexCol">
                {fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
                {(id === 0 || (work && !fetching && fetched)) && 
                    <BaseWork work={work} setWork={setWork} />
                }       
            </div>
        </RightSlider>
    );
}