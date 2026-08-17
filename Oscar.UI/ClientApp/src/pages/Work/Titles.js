import { TextField, FormControl, Card, CardContent, Button, List, ListItem, IconButton } from '@mui/material';
import { Delete, ExpandMore } from '@mui/icons-material';
import React, { useState } from 'react';
import { EnumList } from '../../shared/components/EnumList/EnumList'

const Titles = ({work, setWork}) => {

    const addTitle = () => {
        const newTitle = {title:'', languageCode:'*'};
        const updatedTitles = work.titles? [...work.titles, newTitle]: [newTitle];
        setWork({...work, titles: updatedTitles});
    }

    const removeTitle = (index) => {
        let titles=[...work.titles];
        titles.splice(index,1);
        if(titles.length === 0){
            titles = null;
        }
        setWork({...work, titles});
    }

    const updateTitle = (index, newTitle) => {
        const titles = [...work.titles];
        titles[index] = {...titles[index], title: newTitle};
        setWork({...work, titles});
    }

    const updateTitleLang = (index, newTitleLang) => {
        const titles = [...work.titles];
        titles[index] = {...titles[index], languageCode: newTitleLang};
        setWork({...work, titles});
    }

    const [expanded, setExpanded] = useState(true);

  const toggleAccordion = () => {
    setExpanded(!expanded);
  };

    return (
        <Card elevation={10}>
            <CardContent>
            <h3>Titles</h3>
            <List disablePadding={true}>
                {work.titles?.map((title, index) => { return (
                <ListItem key={index} disablePadding={true} secondaryAction={
                    <IconButton
                        aria-label="delete"
                        color="primary"
                        onClick={() => { removeTitle(index)}}> 
                        <Delete />
                    </IconButton>
                    }>
                <div className="flexRow flexGrow">
                    <div className="inputItem">
                        <TextField
                            label="Title"
                            value={title.title}
                            size="small"
                            variant="standard"
                            fullWidth={true}
                            onChange={(e) => updateTitle(index, e.target.value)}
                            />
                    </div>
                    <div className="inputItem">
                        <FormControl size="small">
                            <EnumList
                                label='Language'
                                uri='/staticData/works/language'
                                value={title.languageCode? title.languageCode.toLowerCase(): -1}
                                keyField ='name'
                                nameField = 'description'
                                nullValue = '-1'
                                onChange={(e) => updateTitleLang(index, e.target.value)}
                                />
                        </FormControl>
                    </div>
                </div>
                </ListItem>)})}
                    
                </List>
                <Button
                    size="small"
                    variant="contained"
                    color="primary"
                    onClick={() => addTitle()}
                >Add title</Button>
            </CardContent>
        </Card>

    );
}

export default Titles;