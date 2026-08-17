import { CircularProgress, Button, IconButton, InputLabel, MenuItem, Select, FormControl} from '@mui/material';
import React, { useEffect, useState } from 'react';
import { toastr } from '../../../shared/helpers/toast';
import { get, create, update, showValidationError } from "../../../shared/helpers/apiaccess"
import BaseWork from '../BaseWork'
import Seasons from '../Seasons'
import Episodes from '../Episodes'
import defaultSeries from '../baseWorkDefault'
import RightSlider from '../../../shared/components/RightSlider/RightSlider';

export default function SeriesDetails({open, id, toggleDrawer, refreshList, discriminator}) {
    defaultSeries.id = id;

    // public ICollection<int>? ClientIds { get; set; }
    // public ICollection<int>? CatalogueIds { get; set; }
    // public ICollection<int>? ProducerIds { get; set; }
    // public ICollection<int>? DirectorIds { get; set; }
    // public ICollection<int>? ActorIds { get; set; }
    // public ICollection<int>? DistributorIds { get; set; }
    // public ICollection<int>? ScreenWriterIds { get; set; }

    //public ICollection<int>? RightIds { get; set; }
    //public ICollection<int>? ConflictIds { get; set; }
    //public ICollection<int>? WorksSubTypeIds { get; set; }
    //public ICollection<int>? CountryIds { get; set; }
    //public ICollection<int>? CompanyIds { get; set; }
    //public ICollection<int>? AlternativeTitleIds { get; set; }
    //public ICollection<int>? LanguageIds { get; set; }


    const [work, setWork] = useState(defaultSeries);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [saving, setSaving] = useState(false);

    const types = [
        { name: "Series", uri: "series"}, 
        { name: "Season", uri: "season"}, 
        { name: "Episode", uri: "episode"},
        { name: "StandAlone", uri: "standAlone"}
    ];

    const [type, setType] = useState( types.find((type) => type.name === discriminator));

    const refreshWork = async() => {
        setFetching(true);
        setFetched(false);
        let retrievedWork = await get(`${getType().uri}/${id}`);
        setWork(retrievedWork);
        setFetching(false);
        setFetched(true);
    }

    const getType = () => {
        return types.find((type) => type.name === discriminator);
    }

    useEffect(() => {
            try {
                setWork(defaultSeries);
                if (id > 0) {
                    refreshWork();
                }
            }
            catch { 
                toastr('error', 'Error retrieving series');
            }

    }, [open]);

    const onClose = () => {
        toggleDrawer();
        setFetching(false);
        setFetched(false);
        refreshList();
    }

    const saveWork = async(successPage) => {
        try {

            // const workToSend = { WorksDto: work };
            setSaving(true);
            let response = work.id ?
            await update(`${getType().uri}/` + work.id, work) :
            await create(type.uri, work);

            const action = id === 0? 'created': 'updated';
            setSaving(false);
            toastr('success', `${getType().name} ${action} successfully`);
            onClose();
        }
        catch (err) {
            setSaving(false);
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error updating ${getType().name}`);
            }
        }
    }

    const addSeason = async() => {
        saveWork('/allseasons/0');
    }

    // const changeType = (value) => {
    //     setType(types.find((type) => type.name ===value));
    // }

    return (
        <RightSlider canSave={true} title={id === 0? `Create ${getType().name}`: ` ${getType().name} Details`} onSave={saveWork}
        onClose={onClose} open={open} toggleDrawer={toggleDrawer} >
            <div className="flexCol">
                {/* <div className="inputItem">
                    <FormControl size="small">
                        <InputLabel>Type</InputLabel>
                        <Select
                            value={type.name}
                            label='Type'
                            onChange={(e) => changeType(e.target.value)}
                        >
                        {types.map((item, index) => (<MenuItem value={item.name} key={index}>{item.name}</MenuItem>))}
                        </Select>
                    </FormControl>
                </div> */}
                {fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
                {(id === 0 || (work && !fetching && fetched)) && 
                    <BaseWork work={work} setWork={setWork} />
                }
                {(id > 0 && discriminator === 'Series' && (work && !fetching && fetched)) &&
                    <Seasons work={work} setWork={setWork} refreshSeries={refreshWork}/>
                }
                {(id > 0  && discriminator === 'Season' && (work && !fetching && fetched)) &&
                    <Episodes work={work} setWork={setWork} refreshSeries={refreshWork}/>
                }
                
            </div>
        </RightSlider>
    );
}