import { TextField, CircularProgress, Button, Input } from '@mui/material';
import React, { useEffect, useState } from 'react';
import { toastr } from '../../shared/helpers/toast';
import RightSlider from '../../shared/components/RightSlider/RightSlider';
import { get, create, update, showValidationError } from "../../shared/helpers/apiaccess";
import { AutoComplete } from '../../shared/components/AutoComplete/AutoComplete';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import moment from 'moment';

export default function MatchDetails({open, id, toggleDrawer, refreshList}) {

    const defaultMatch = {
        rules: null,
        formFile: null,
        requestedBy: '',
        clientId: null,
        territoryId: -1,
        rightsTypeId: null,
        productionYear: null,
        rightsFromYear: null,
        rightsToYear: null,
        ignoreCharactersFollowing: '',
        id: id
    }
    
    const [match, setMatch] = useState(defaultMatch);
    const [fetching, setFetching] = useState(false);
    const [fetched, setFetched] = useState(false);
    const [filename, setFilename] = useState('');

    const fontColor = {
        style: { color: 'rgb(50, 50, 50)' }
    }

    useEffect(() => {
        (async () => {
            try {
                setMatch(defaultMatch);
                setFilename('');
                if (open && id > 0) {
                    setFetching(true);
                    setFetched(false);
                    let retrievedMatch = await get(`matchRequest/get/${id}`);
                    setMatch(retrievedMatch);
                    setFetching(false);
                    setFetched(true);
                }
            }
            catch { 
                toastr('error', 'Error retrieving match');
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

    const changeRules = (selectedRules) => {
        const changedRules = selectedRules.map(rule => (rule.key));
        setMatch({...match, rules: changedRules.toString()});
    }

    const changeTerritory = (selectedTerritory) => {
        setMatch({...match, territoryId: selectedTerritory});
    }

    const changeFile = (e) => {
        let name = '';
        if(e.target.files[0]){
            name = e.target.files[0].name;
            setMatch({...match, formFile: e.target.files[0]});
        }
        setFilename(name);
    }

    async function saveMatch() {
        try {

            const matchToSend = {...match};

            let response = match.id ?
            await update('matchRequest/put/' + match.id, matchToSend) :
            await create('matchRequest/post', matchToSend, true);

            const action = id === 0? 'created': 'updated';
            toastr('success', `Match ${action} successfully`);
            onClose();
        }
        catch (err) {
            if(err.response && err.response.status === 400 && err.response.data && err.response.data.errorMessage){
              showValidationError(err.response.data.errorMessage);
            }
            else{
                toastr('error', `Error creating match`);
            }
        }
    }

    return (
        <RightSlider canSave={true} title={id === 0? "Create Match": "Match Details"} onSave={saveMatch}
        onClose={onClose} open={open} toggleDrawer={toggleDrawer} >
            <div className="flexCol">
                {fetching && <div className="loaderIcon"><CircularProgress size={40} /></div>}
                {(id === 0 || (match && !fetching && fetched)) && (
                <>
                <div className="inputItem">
                    <AutoComplete
                        multiple
                        label='Match Rules'
                        uri='/staticData/matching/rules'
                        keyField ='name'
                        nameField = 'name'
                        onChange={(e, selectedValues) => changeRules(selectedValues)}
                    />
                </div>
                <div className="inputItem">
                    <TextField
                        fullWidth={true}
                        label="Requested by"
                        size="small"
                        variant="standard"
                        value={match.requestedBy}
                        inputProps={fontColor}
                        onChange={(e) => setMatch({...match, requestedBy: e.target.value})}
                        />
                </div>
                <div className="inputItem">
                <label htmlFor="contained-button-file">
                    <Input inputProps={{ accept: 'text/csv' }} id="contained-button-file" type="file" hidden
                    onChange={(e) => changeFile(e)} />
                    <Button variant="contained" component="span">
                        Select csv file
                    </Button>
                    <span> {filename}</span>
                    </label>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <DatePicker
                            views={["year"]}
                            label="Production Year"
                            size="small"
                            variant="standard"
                            value={match.productionYear}
                            inputProps={fontColor}
                            onChange={(newValue) => setMatch({...match, productionYear: moment(newValue).format('YYYY')})}
                            renderInput={(params) => <TextField {...params} helperText={null} />}
                            />
                    </div>
                    <div className="inputItem">
                        <DatePicker
                            views={["year"]}
                            label="Rights From Year"
                            size="small"
                            variant="standard"
                            value={match.rightsFromYear}
                            inputProps={fontColor}
                            onChange={(newValue) => setMatch({...match, rightsFromYear: moment(newValue).format('YYYY')})}
                            renderInput={(params) => <TextField {...params} helperText={null} />}
                            />
                    </div>
                    <div className="inputItem">
                        <DatePicker
                            views={["year"]}
                            label="Rights To Year"
                            size="small"
                            variant="standard"
                            value={match.rightsToYear}
                            inputProps={fontColor}
                            onChange={(newValue) => setMatch({...match, rightsToYear: moment(newValue).format('YYYY')})}
                            renderInput={(params) => <TextField {...params} helperText={null} />}
                            />
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <AutoComplete
                            label='Territory'
                            uri='/staticData/country/all'
                            value={match.territoryId}
                            keyField ='id'
                            nameField = 'name'
                            onChange={(e, selctedValue) => setMatch({...match, territoryId: selctedValue.id})}
                        />
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <AutoComplete
                            label='Client'
                            uri='/client/basic'
                            value={match.clientId}
                            keyField ='id'
                            nameField = 'clientName'
                            onChange={(e, selctedValue) => setMatch({...match, clientId: selctedValue.id})}
                        />
                    </div>
                </div>
                <div className="inputItem">
                    <TextField
                        fullWidth={true}
                        label="Ignore characters following"
                        size="small"
                        variant="standard"
                        value={match.ignoreCharactersFollowing}
                        inputProps={fontColor}
                        onChange={(e) => setMatch({...match, ignoreCharactersFollowing: e.target.value})}
                        />
                </div>
                </>
                )}
            </div>
        </RightSlider>
    );
}