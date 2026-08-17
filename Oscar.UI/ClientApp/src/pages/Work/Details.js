import { TextField, CircularProgress, InputLabel,
    MenuItem, Select, FormControl, Card, CardContent, Button, List, ListItem, IconButton, ListItemText } from '@mui/material';
import { Delete, Add } from '@mui/icons-material';
import React, { useEffect, useState } from 'react';
import { isEqual, isEmpty } from 'lodash';
import { toastr } from '../../shared/helpers/toast';
import RightSlider from '../../shared/components/RightSlider/RightSlider';
import { get, create, update, showValidationError } from "../../shared/helpers/apiaccess"
import { EnumList } from '../../shared/components/EnumList/EnumList'
import Titles from './Titles';

export default function Details({open, id, toggleDrawer, refreshList}) {
    const defaultWork = {
        type: 'episode',
        worksStatus: null,
        genreId: 1,
        reference: '',
        durationMinutes: 0,
        productionYear: null,
        firstBroadcastYear: null,
        iMaestroWorkCode: null,
        agicoaDeclarationNumber: null,
        isan: null,
        cavcoCtcCode: null,
        generalNotes: '',
        number: null,
        titles:[{title:'name1', languageCode:'eng'}, {title:'name2', languageCode:'fre'}],
        id: id
    }
    
    const [work, setWork] = useState(defaultWork);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);

    const fontColor = {
        style: { color: 'rgb(50, 50, 50)' }
    }

    useEffect(() => {
        (async () => {
            try {
                setWork(defaultWork);
                if (open && id > 0) {
                    setFetching(true);
                    setFetched(false);
                    let retrievedWork = await get(`works/get/${id}`);
                    setWork(retrievedWork);
                    setFetching(false);
                    setFetched(true);
                }
            }
            catch { 
                toastr('error', 'Error retrieving work');
                onClose();
            }
        })();
    }, [open]);

    function onClose() {
        toggleDrawer();
        setFetching(false);
        setFetched(false);
        refreshList();
    }

    const changeType = () => {

    };

    async function saveWork() {
        try {

            // const workToSend = { WorksDto: work };

            let response = work.id ?
            await update('works/put/' + work.id, work) :
            await create(`${work.type}`, work);

            const action = id === 0? 'created': 'updated';
            toastr('success', `Work ${action} successfully`);
            onClose();

        }
        catch (err) {
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error updating work`);
            }
        }
    }

    return (
        <RightSlider canSave={true} title={id === 0? "Create Work": "Work Details"} onSave={saveWork}
        onClose={onClose} open={open} toggleDrawer={toggleDrawer} >
            <div className="flexCol">
                {fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
                {(id === 0 || (work && !fetching && fetched)) && (
                <>
                <div className="inputItem">
                    <FormControl size="small">
                        <InputLabel id="typeLabel">Type</InputLabel>
                        <Select
                            value={work.type}
                            labelId="typeLabel"
                            label="Type"
                            onChange={(e) => setWork({...work, type: e.target.value})}
                        >
                        <MenuItem value="episode" key="episode">Episode</MenuItem>
                        <MenuItem value="series" key="Series">Series</MenuItem>
                        <MenuItem value="season" key="Season">Season</MenuItem>
                        <MenuItem value="standalone" key="standAlone">Stand-alone</MenuItem>
                        </Select>
                    </FormControl>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Reference"
                        size="small"
                        variant="standard"
                        value={work.reference}
                        inputProps={fontColor}
                        onChange={(e) => setWork({...work, reference: e.target.value})}
                        />
                </div>
                <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Genre'
                            uri='/staticData/works/genre'
                            value={work.genreId? work.genreId: -1}
                            keyField ='id'
                            nameField = 'description'
                            nullValue = '-1'
                            onChange={(e) => setWork({...work, genreId: e.target.value})}
                            />
                    </FormControl>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Duration (mins)"
                        size="small"
                        variant="standard"
                        type="number"
                        value={work.durationMinutes}
                        inputProps={fontColor}
                        onChange={(e) => setWork({...work, durationMinutes: e.target.value})}
                        />
                </div>
                <div className="inputItem">
                    <Titles work={work} setWork={setWork}></Titles>
                </div>
                {/* <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Status'
                            uri='/staticData/client/statuses'
                            value={client.status? client.status: -1}
                            keyField ='name'
                            nameField = 'name'
                            nullValue = '-1'
                            onChange={(e) => setClient({...client, status: e.target.value})}
                            />
                    </FormControl>
                </div>
                <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Type'
                            uri='/staticData/client/types'
                            value={client.clientType? client.clientType: -1}
                            keyField ='key'
                            nameField = 'name'
                            nullValue = '-1'
                            onChange={(e) => setClient({...client, clientType: e.target.value})}
                            />
                    </FormControl>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Email"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.email}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, email: e.target.value})}
                        />
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                            label="iMaestro Client Code"
                            fullWidth={true}
                            size="small"
                            variant="standard"
                            value={client.iMaestroClientCode}
                            inputProps={fontColor}
                            onChange={(e) => setClient({...client, iMaestroClientCode: e.target.value})}
                            />
                    </div>
                    <div className="inputItem">
                        <TextField
                            type="number"
                            label="Contract id"
                            fullWidth={true}
                            size="small"
                            variant="standard"
                            value={client.contractId? client.contractId: undefined}
                            inputProps={fontColor}
                            onChange={(e) => setClient({...client, contractId: e.target.value})}
                            />
                    </div>
                </div>
                <div className="inputItem">
                    <TextField
                        label="Address line 1"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.address.addressLine1}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, address: {...client.address, addressLine1: e.target.value}})}
                        />
                </div>
                <div className="inputItem">
                    <TextField
                        label="Address line 2"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.address.addressLine2}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, address: {...client.address, addressLine2: e.target.value}})}
                        />
                </div>
                <div className="inputItem">
                    <TextField
                        label="Address line 3"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.address.addressLine3}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, address: {...client.address, addressLine3: e.target.value}})}
                        />
                </div>
                <div className="inputItem">
                    <TextField
                        label="Post / Zip Code"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.address.postZipCode}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, address: {...client.address, postZipCode: e.target.value}})}
                        />
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <FormControl size="small">
                            <EnumList
                                label='Country'
                                uri='/staticData/country/all'
                                value={isEmpty(client.address.country)? ' ': client.address.country}
                                keyField ='name'
                                nameField = 'name'
                                nullValue = ' '
                                onChange={(e) => setClient({...client, address: {...client.address, country: e.target.value}})}
                                />
                        </FormControl>
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Grade'
                            uri='/staticData/client/grades'
                            value={client.clientGrade? client.clientGrade: -1}
                            keyField ='name'
                            nameField = 'name'
                            nullValue = '-1'
                            onChange={(e) => setClient({...client, clientGrade: e.target.value})}
                         />
                    </FormControl>
                    </div>
                </div>
                <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Status'
                            uri='/staticData/client/statuses'
                            value={client.status? client.status: -1}
                            keyField ='name'
                            nameField = 'name'
                            nullValue = '-1'
                            onChange={(e) => setClient({...client, status: e.target.value})}
                            />
                    </FormControl>
                </div>
                <div className="inputItem">
                    <FormControl size="small">
                        <EnumList
                            label='Type'
                            uri='/staticData/client/types'
                            value={client.clientType? client.clientType: -1}
                            keyField ='key'
                            nameField = 'name'
                            nullValue = '-1'
                            onChange={(e) => setClient({...client, clientType: e.target.value})}
                            />
                    </FormControl>
                </div>
                <div className="inputItem">
                    <TextField
                        multiline={true}
                        rows={4}
                        label="General notes"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={client.generalNotes}
                        inputProps={fontColor}
                        onChange={(e) => setClient({...client, generalNotes: e.target.value})}
                        />
                </div> */}
                </>
                )}
            </div>
        </RightSlider>
    );
}