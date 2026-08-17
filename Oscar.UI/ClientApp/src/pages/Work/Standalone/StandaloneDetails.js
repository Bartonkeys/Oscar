import { CircularProgress} from '@mui/material';
import React, { useEffect, useState } from 'react';
import { toastr } from '../../../shared/helpers/toast';
import { get, create, update, showValidationError } from "../../../shared/helpers/apiaccess"
import BaseWork from '../BaseWork'
import defaultStandalone from '../baseWorkDefault'
import RightSlider from '../../../shared/components/RightSlider/RightSlider';

export default function StandaloneDetails({open, id, seriesId, toggleDrawer, onSuccessfulSave}) {
    defaultStandalone.id = id;

    const [work, setWork] = useState(defaultStandalone);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [saving, setSaving] = useState(false);

    const refreshWork = async() => {
        setFetching(true);
        setFetched(false);
        let retrievedWork = await get(`standAlone/${id}`);
        setWork(retrievedWork);
        setFetching(false);
        setFetched(true);
    }

    useEffect(() => {
        (async () => {
            try {
                setWork(defaultStandalone);
                if (id > 0) {
                    refreshWork();
                }
            }
            catch { 
                toastr('error', 'Error retrieving standalone work');
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

            // const workToSend = { WorksDto: work };
            setSaving(true);
            let response = work.id ?
            await update('standAlone/' + work.id, work) :
            await create('standAlone', work);

            const action = id === 0? 'created': 'updated';
            setSaving(false);
            toastr('success', `Standalone work ${action} successfully`);
            onSuccessfulSave();
            onClose();
        }
        catch (err) {
            setSaving(false);
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error updating standalone work`);
            }
        }
    }

    return (
        <RightSlider canSave={true} title={id === 0? "Create Standalone": "Standalone Details"} onSave={saveWork}
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