import { Button, List, ListItem, IconButton, Card, CardContent } from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import React, { useState } from 'react';
import { isEmpty } from 'lodash';
import EpisodeDetails from './Episode/EpisodeDetails'
import ConfirmPopover from '../../shared/components/ConfirmPopover/ConfirmPopover';
import { remove } from "../../shared/helpers/apiaccess"

const Episodes = ({work, setWork, onSuccessfulSave}) => {
    console.log('work: ', work);
    const [openEdit, setOpenEdit] = useState(false);
    const [openConfirmModal, setOpenConfirmModal] = useState(false);
    const [episodeToRemove, setEpisodeToRemove] = useState();
    const [id, setId] = useState(0);

    const removeEpisode = async(removeId) => {
        await remove(`episode/${removeId}`);
        await onSuccessfulSave();
    }

    const showRemoveEpisodePopup = (episode) => {
        setEpisodeToRemove(episode);
        setOpenConfirmModal(true);
    }

    const editEpisode = (id) => {
        setId(id);
        setOpenEdit(true);
    }

    return (
        <div className="inputItem">
            {episodeToRemove &&
                <ConfirmPopover
                open={openConfirmModal}
                question={`Are you sure you want to remove episode "${episodeToRemove.titles[0].title}"?`}
                title='Remove episode'
                action={() => { removeEpisode(episodeToRemove.id) }}
                closeModel={() => { setOpenConfirmModal(false) }}
                color='primary'
        />

            }
            <Card elevation={10}>
                <CardContent>
                <h3>Episodes</h3>
                {!isEmpty(work.episodes) && 
                    <List disablePadding={true}>
                        {work.episodes.map((episode, index) => { return (
                        <ListItem
                            style={{backgroundColor: '#EEEEEE', marginBottom:'3px'}}
                            key={index}
                            disablePadding={true}
                            secondaryAction={
                                <span>
                                    <IconButton
                                        aria-label="edit"
                                        color="primary"
                                        onClick={() => { editEpisode(episode.id)}}> 
                                        <Edit />
                                    </IconButton>
                                    <IconButton
                                        aria-label="delete"
                                        color="primary"
                                        onClick={() => { showRemoveEpisodePopup(episode)}}> 
                                        <Delete />
                                    </IconButton>
                                    
                                </span>
                            }>
                        <div className="flexRow flexGrow">
                            <div className="inputItem">
                                {episode.titles[0].title}
                            </div>
                        </div>
                        </ListItem>)})}
                    </List>
                }       
                <Button
                    size="small"
                    variant="contained"
                    color="primary"
                    onClick={() => {editEpisode(0)}}
                >Add episode</Button>
                </CardContent>
            </Card>
            <EpisodeDetails
                open={openEdit}
                toggleDrawer={setOpenEdit}
                id={id}
                seasonId={work.discriminator==='Season'? work.id: null}
                seriesId={work.discriminator==='Series'? work.id: null}
                onSuccessfulSave={onSuccessfulSave}
            />
      </div>
    );
}

export default Episodes;
