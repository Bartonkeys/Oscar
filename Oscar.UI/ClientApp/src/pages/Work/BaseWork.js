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
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import moment from 'moment';
import { ListBuilder } from '../../shared/components/ListBuilder/ListBuilder'

export default function BaseWork({work, setWork}) {
    return (
            <div className="flexCol">
                <div className="inputItem">
                    <Titles work={work} setWork={setWork} />
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <FormControl size="small">
                            <EnumList
                                label='Status'
                                uri='/staticData/works/status'
                                value={work.worksStatus? work.worksStatus: 'None'}
                                keyField ='name'
                                nameField = 'name'
                                nullValue = 'None'
                                onChange={(e) => setWork({...work, worksStatus: e.target.value})}
                                />
                        </FormControl>
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
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                        fullWidth={true}
                            label="Duration (mins)"
                            size="small"
                            variant="standard"
                            type="number"
                            value={work.durationMinutes}
                            onChange={(e) => setWork({...work, durationMinutes: e.target.value})}
                            />
                    </div>
                    <div className="inputItem">
                        <TextField
                        fullWidth={true}
                            label="Number"
                            size="small"
                            variant="standard"
                            type="number"
                            value={work.number}
                            onChange={(e) => setWork({...work, number: e.target.value})}
                            />
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <DatePicker
                            views={["year"]}
                            label="Production Year"
                            size="small"
                            variant="standard"
                            value={work.productionYear.toString()}
                            onChange={(newValue) => setWork({...work, productionYear: moment(newValue).format('YYYY')})}
                            renderInput={(params) => <TextField {...params} helperText={null} />}
                            />
                    </div>
                    <div className="inputItem">
                        <DatePicker
                            views={["year"]}
                            label="First Broadcast Year"
                            size="small"
                            variant="standard"
                            value={work.firstBroadcastYear.toString()}
                            onChange={(newValue) => setWork({...work, firstBroadcastYear: moment(newValue).format('YYYY')})}
                            renderInput={(params) => <TextField {...params} helperText={null} />}
                            />
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="iMaestro Work Code"
                            size="small"
                            variant="standard"
                            value={work.iMaestroWorkCode}
                            onChange={(e) => setWork({...work, iMaestroWorkCode: e.target.value})}
                            />
                    </div>
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="Agicoa Declaration Number"
                            size="small"
                            variant="standard"
                            value={work.agicoaDeclarationNumber}
                            onChange={(e) => setWork({...work, agicoaDeclarationNumber: e.target.value})}
                            />
                    </div>
                </div>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="ISAN"
                            size="small"
                            variant="standard"
                            value={work.isan}
                            onChange={(e) => setWork({...work, isan: e.target.value})}
                            />
                    </div>
                    <div className="inputItem">
                        <TextField
                            fullWidth={true}
                            label="Cavco CTC Code"
                            size="small"
                            variant="standard"
                            value={work.cavcoCtcCode}
                            onChange={(e) => setWork({...work, cavcoCtcCode: e.target.value})}
                            />
                    </div>
                </div>
                <div className="inputItem">
                    <TextField
                        fullWidth={true}
                        multiline={true}
                        rows={4}
                        label="General notes"
                        fullWidth={true}
                        size="small"
                        variant="standard"
                        value={work.generalNotes}
                        onChange={(e) => setWork({...work, generalNotes: e.target.value})}
                        />
                </div>
                <div className="inputItem">
                    {/* <ListBuilder
                        title ='Actor'
                        displayField: PropTypes.string.isRequired,
                        onChange: PropTypes.func.isRequired,
                        client: PropTypes.string,
                        current: PropTypes.array,
                        disabled: PropTypes.bool,
                        fetchUri: PropTypes.string,
                        displayFunc: PropTypes.func,
                        returnFunc: PropTypes.func,
                        childFunc: PropTypes.func
                        /> */}
                </div>

            </div>
    );
}
