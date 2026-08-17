import { TextField, CircularProgress, FormControl, InputLabel } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { isEqual, isEmpty } from 'lodash';
import { toastr } from '../../shared/helpers/toast';
import RightSlider from '../../shared/components/RightSlider/RightSlider';
import { get, create, update, showValidationError } from "../../shared/helpers/apiaccess"
import {EnumList} from '../../shared/components/EnumList/EnumList'

export default function ClientDetails({open, id, toggleDrawer, refreshList}) {
    const defaultAddress = {
        addressLine1: '',
        addressLine2: '',
        addressLine3: '',
        postZipCode: '',
        country: ' '
    };
    const defaultClient = {
        clientName: '',
        status: -1,
        clientReference: '',
        clientGrade: -1,
        clientType: -1,
        iMaestroClientCode: '',
        email: '',
        generalNotes: '',
        address: defaultAddress,
        id: id
    }
    
    const [client, setClient] = useState(defaultClient);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);

    const fontColor = {
        style: { color: 'rgb(50, 50, 50)' }
    }

    useEffect(() => {
        (async () => {
            try {
                setClient(defaultClient);
                if (open && id > 0) {
                    setFetching(true);
                    setFetched(false);
                    let retrievedClient = await get(`client/get/${id}`);
                    if(!retrievedClient.address){
                        retrievedClient.address = defaultClient.address;
                    }
                    setClient(retrievedClient);
                    setFetching(false);
                    setFetched(true);
                }
            }
            catch { 
                toastr('error', 'Error retrieving client');
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

    async function saveClient() {
        try {

            // const isAddressEmpty = every(client.address, v => isEmpty(v));
            const isAddressEmpty = isEqual(client.address, defaultAddress);
            
            const clientToSend = {...client, address: isAddressEmpty? null: client.address};

            let response = client.id ?
            await update('client/put/' + client.id, clientToSend) :
            await create('client/create', clientToSend);

            const action = id === 0? 'created': 'updated';
            toastr('success', `Client ${action} successfully`);
            onClose();

        }
        catch (err) {
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error updating client`);
            }
        }
    }

    return (
        <RightSlider canSave={true} title={id === 0? "Create Client": "Client Details"} onSave={saveClient}
        onClose={onClose} open={open} toggleDrawer={toggleDrawer} >
            <div className="flexCol">
                {fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
                {(id === 0 || (client && !fetching && fetched)) && (
                <>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                            label="Name"
                            fullWidth={true}
                            size="small"
                            variant="standard"
                            value={client.clientName}
                            inputProps={fontColor}
                            onChange={(e) => setClient({...client, clientName: e.target.value})}
                            />         
                    </div>
                    <div className="inputItem">
                        <TextField
                            label="Reference"
                            size="small"
                            variant="standard"
                            value={client.clientReference}
                            inputProps={fontColor}
                            onChange={(e) => setClient({...client, clientReference: e.target.value})}
                            />
                    </div>
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
                </div>
                </>
                )}
            </div>
        </RightSlider>
    );
}