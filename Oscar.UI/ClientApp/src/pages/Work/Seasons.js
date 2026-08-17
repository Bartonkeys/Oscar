import { Button, List, ListItem, IconButton, Card, CardContent } from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import React, { useState } from 'react';
import { isEmpty } from 'lodash';
import SeasonDetails from './Season/SeasonDetails'
import ConfirmPopover from '../../shared/components/ConfirmPopover/ConfirmPopover';
import { remove } from "../../shared/helpers/apiaccess"

const Seasons = ({work, setWork, refreshSeries}) => {
    const [openEdit, setOpenEdit] = useState(false);
    const [openConfirmModal, setOpenConfirmModal] = useState(false);
    const [seasonToRemove, setSeasonToRemove] = useState();
    const [id, setId] = useState(0);

    const removeSeason = async(removeId) => {
        await remove(`season/${removeId}`);
        await refreshSeries();
    }

    const showRemoveSeasonPopup = (season) => {
        setSeasonToRemove(season);
        setOpenConfirmModal(true);
    }

    const editSeason = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="inputItem">
            {seasonToRemove && 
                <ConfirmPopover
                    open={openConfirmModal}
                    question={`Are you sure you want to remove season "${seasonToRemove.titles[0].title}"?`}
                    title='Remove season'
                    action={() => { removeSeason(seasonToRemove.id) }}
                    closeModel={() => { setOpenConfirmModal(false) }}
                    color='primary'
                />
            }
            <Card elevation={10}>
                <CardContent>
                <h3>Seasons</h3>
                {!isEmpty(work.seasons) && 
                    <List disablePadding={true}>
                        {work.seasons.map((season, index) => { return (
                        <ListItem
                            style={{backgroundColor: '#EEEEEE', marginBottom:'3px'}}
                            key={index}
                            disablePadding={true}
                            secondaryAction={
                                <span>
                                    <IconButton
                                        aria-label="edit"
                                        color="primary"
                                        onClick={() => { editSeason(season.id)}}> 
                                        <Edit />
                                    </IconButton>
                                    <IconButton
                                        aria-label="delete"
                                        color="primary"
                                        onClick={() => { showRemoveSeasonPopup(season)}}>  
                                        <Delete />
                                    </IconButton>
                                    
                                </span>
                            }>
                        <div className="flexRow flexGrow">
                            <div className="inputItem">
                                {season.titles[0].title}
                            </div>
                        </div>
                        </ListItem>)})}
                    </List>
                }       
                <Button
                    size="small"
                    variant="contained"
                    color="primary"
                    onClick={() => {editSeason(0)}}
                >Add season</Button>
                </CardContent>
            </Card>
            <SeasonDetails open={openEdit} toggleDrawer={setOpenEdit} id={id} seriesId={work.id} onSuccessfulSave={refreshSeries}/>
      </div>
    );
}

export default Seasons;
