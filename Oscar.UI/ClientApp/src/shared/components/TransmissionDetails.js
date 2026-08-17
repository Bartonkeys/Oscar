import React, { useEffect, useState } from "react";
import { canEdit, getMritClient } from "../helpers/client";
import { useParams } from "react-router-dom";
import { makeStyles } from "@mui/material/styles";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Typography from "@mui/material/Typography";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";

import { Button, Drawer, IconButton } from "@mui/material/";
import history from "../helpers/history";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Badge from "@mui/material/Badge";
import "./TransmissionDetails.css";
import { Box } from "@mui/material";
import Moment from "react-moment";
import { Close } from "@mui/icons-material";

import { createTheme , ThemeProvider } from "@mui/material/styles";

const useStyles = makeStyles((theme) => ({
  palette: {},

  root: {
    overflowX: "hidden",
    padding: "10px",
  },

  column: {
    flexGrow: 1,
  },

  paper: {
    padding: theme.spacing(0.5, 1.5, 1),
    height: theme.spacing(9),
    textAlign: "left",
    color: theme.palette.text.secondary,
  },

  heading: {
    fontSize: theme.typography.pxToRem(18),
    fontWeight: theme.typography.fontWeightMedium,
    gutterBottom: true,
    width: 500,
    text: {
      secondary: "ff7961",
    },
  },
  contents: {
    width: "100%",
    fontSize: theme.typography.pxToRem(15),
    fontWeight: theme.typography.fontWeightRegular,
  },

  list: {},
  fullList: {
    width: "auto",
  },
}));
const theme = createTheme({
  typography: {
    subtitle2: {
      fontSize: 12,
    },
    h6: {
      text: {
        primary: "#000",
      },
      fontSize: 17,
    },
    body1: {
      fontWeight: 500,
    },
  },
});

export default function TransmissionDetails(props) {
  const classes = useStyles();
  const [transmissionDetails, setTransmissionDetails] = useState({});
  const [drawerState, setDrawState] = useState({
    open: false,
    initial: true,
    hasChanged: false
  });
  const { id } = useParams();
  const routeState = props?.location?.state;

  //call the api and update state with new props

  useEffect(() => {
    if (drawerState.initial) {
      (async () => {
        try {
          let response = await getMritClient().get("transmission/" + id + "/detail");
          setTransmissionDetails(response.data);
          setDrawState({ ...drawerState, open: true, initial: false });
        }
        catch {
          history.goBack();
        }
      })();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [drawerState.initial]);

  function closeDrawer() {
    setDrawState({ ...drawerState, open: false });
    setTimeout(() => {
      if (drawerState.hasChanged && routeState?.returnUrl) {
        history.replace(routeState.returnUrl, routeState?.returnState);
      }
      else {
        history.goBack()
      }
    }, 300);
  }

  function removeUncertainMatch() {
    (async () => {
      try {
        let response = await getMritClient().patch('transmission/' + id + '/remove/low/probability');

        if (response.status === 204) {
          setTransmissionDetails({ ...transmissionDetails, lowMatchProbability: false });
          setDrawState({ ...drawerState, hasChanged: true });
        }
      } catch { }
    })();
  }

  return (
    <React.Fragment key="right">
      <ThemeProvider theme={theme}>
        <Drawer
          anchor="right"
          open={drawerState.open}
          onClose={closeDrawer}>
          <div
            className={
              drawerState.open ? "transmissionDrawer" : "transmissionDrawer"}>
            <div className="flexRow m-3">
              <div>
                <h3 className={classes.transmisionDetailsheader} style={{ fontSize: "24px" }}>Transmission Details</h3>
              </div>

              <div className="moveRight">
                <IconButton onClick={closeDrawer} ><Close /></IconButton>
              </div>
            </div>
            {canEdit() && transmissionDetails?.lowMatchProbability ?
              <div className="flexRight flexRow mr-3">
                <div className="matchPill">
                  <div className="minorText">
                    <div>Uncertain Match Tag</div>
                  </div>
                </div>
                <Button variant="contained" color="secondary" onClick={removeUncertainMatch} style={{ borderTopLeftRadius: 0, borderBottomLeftRadius: 0 }}>Remove</Button>
              </div>
              : <div></div>}

            <div className="flexCol p-3 flexGrow scrollVertical">
              {transmissionDetails ? (
                <div className={classes.root} align="left">
                  <div className={classes.column}>
                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper} >
                          <Typography component={'span'} variant="h6">Production Title </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {" "}
                            {transmissionDetails?.productionTitle}
                          </Typography>
                        </Paper>
                      </Grid>

                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Episode Title</Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.episodeTitle}
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>
                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Broadcast Date </Typography>
                          <Typography variant="body1" color="textPrimary">
                            <Moment format="DD/MM/YYYY">
                              {transmissionDetails.broadcastDateTime}
                            </Moment>
                          </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Broadcast Time </Typography>
                          <Typography variant="body1" color="textPrimary">
                            <Moment format="hh:mm A">
                              {transmissionDetails.broadcastDateTime}
                            </Moment>
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>
                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Duration</Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.broadcastDuration} {" "}
                          min
                        </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Territories </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.channel?.regions?.join(", ")}
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>

                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">
                            Broadcast Language{" "}
                          </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.language?.englishName}
                          </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">
                            Production Language{" "}
                          </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.productionLanguages?.map(x => x.englishName)?.join(', ')}
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>

                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Channel</Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.channel?.name}
                          </Typography>
                        </Paper>
                      </Grid>

                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">
                            Production Year{" "}
                          </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.productionYear}
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>


                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">Series Number</Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.seriesNumber}
                          </Typography>
                        </Paper>
                      </Grid>

                      <Grid item xs={6}>
                        <Paper className={classes.paper}>
                          <Typography component={'span'} variant="h6">
                            Episode Number{" "}
                          </Typography>
                          <Typography variant="body1" color="textPrimary">
                            {transmissionDetails?.episodeNumber}
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>
                  </div>
                  <div className="mb-3"></div>

                  {transmissionDetails?.actors?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel1a-content"
                        id="panel1a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary">
                          Actors
                        <Box mr={53.5} display="inline"></Box>
                          <Badge
                            badgeContent={transmissionDetails?.actors?.length}
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.actors?.map((actor) => (
                            <li key={actor.id} className="TransmisionDetails">
                              {actor.forename} {actor.middleNames} {actor.surname}{" "}
                            </li>
                          ))}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Actors
                        <Box mr={53.5} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.directors?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Directors
                        <Box mr={51} display="inline"></Box>
                          <Badge
                            badgeContent={transmissionDetails?.directors?.length}
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>

                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.directors?.map((director) => (
                            <li key={director.id} className="TransmisionDetails">
                              {director.forename} {director.middleNames}{" "}
                              {director.surname}{" "}
                            </li>
                          ))}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Directors
                        <Box mr={51} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.producers?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textPrimary"
                        >
                          Producers
                        <Box mr={49.8} display="inline"></Box>
                          <Badge
                            badgeContent={transmissionDetails?.producers?.length}
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>

                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.producers?.map((producer) => (
                            <li key={producer.id} className="TransmisionDetails">
                              {producer.forename} {producer.middleNames}{" "}
                              {producer.surname}{" "}
                            </li>
                          ))}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel2a-content"
                          id="panel2a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Producers
                        <Box mr={49.8} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.genres?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Genres
                        <Box mr={53} display="inline"></Box>
                          <Badge
                            badgeContent={transmissionDetails?.genres?.length}
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.genres?.map((genre) => (
                            <li key={genre.id} className="TransmisionDetails">{genre.name} </li>
                          ))}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Genres
                        <Box mr={53} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.altProductionTitles?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Alternate Production Title
                        <Box mr={34.3} display="inline"></Box>
                          <Badge
                            badgeContent={
                              transmissionDetails?.altProductionTitles?.length
                            }
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.altProductionTitles?.map(
                            (altProductionTitle) => (
                              <li key={id + altProductionTitle} className="TransmisionDetails">{altProductionTitle} </li>
                            )
                          )}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Alternate Production Title
                        <Box mr={34.3} display="inline"></Box>
                            <Badge>
                              <Badge
                                badgeContent={0} showZero
                                color="primary" >
                              </Badge>
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.altEpisodeTitles?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Alternate Episode Title
                        <Box mr={37.5} display="inline"></Box>
                          <Badge
                            badgeContent={
                              transmissionDetails?.altEpisodeTitles?.length
                            }
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.altEpisodeTitles?.map(
                            (altEpisodeTitle) => (
                              <li key={id + altEpisodeTitle} className="TransmisionDetails">{altEpisodeTitle} </li>
                            )
                          )}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Alternate Episode Title
                        <Box mr={37.5} display="inline"></Box>
                            <Badge>
                              <Badge
                                badgeContent={0} showZero
                                color="primary" >
                              </Badge>
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.productionCompanies?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Production Companies
                        <Box mr={37.2} display="inline"></Box>
                          <Badge
                            badgeContent={
                              transmissionDetails?.productionCompanies?.length
                            }
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.productionCompanies?.map(
                            (productionCompany) => (
                              <li key={productionCompany.id} className="TransmisionDetails">{productionCompany.name} </li>
                            )
                          )}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Production Companies
                        <Box mr={37.2} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}

                  {transmissionDetails?.countriesOfOrigin?.length !== 0 ? (
                    <Accordion>
                      <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        aria-controls="panel2a-content"
                        id="panel2a-header"
                      >
                        <Typography component={'span'}
                          className={classes.heading}
                          color="textSecondary"
                        >
                          Countries of Origin
                        <Box mr={41.5} display="inline"></Box>
                          <Badge
                            badgeContent={
                              transmissionDetails?.countriesOfOrigin?.length
                            }
                            color="primary"
                          ></Badge>
                        </Typography>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Typography component={'span'} className={classes.contents} align="left">
                          {transmissionDetails?.countriesOfOrigin?.map(
                            (countriesOfOrigin) => (
                              <li key={countriesOfOrigin.id} className="TransmisionDetails">{countriesOfOrigin.name} </li>
                            )
                          )}
                        </Typography>
                      </AccordionDetails>
                    </Accordion>
                  ) : (
                      <Accordion disabled>
                        <AccordionSummary
                          expandIcon={<ExpandMoreIcon />}
                          aria-controls="panel3a-content"
                          id="panel3a-header"
                        >
                          <Typography component={'span'} className={classes.heading}>
                            Countries of Origin
                        <Box mr={41.5} display="inline"></Box>
                            <Badge
                              badgeContent={0} showZero
                              color="primary" >
                            </Badge>
                          </Typography>
                        </AccordionSummary>
                      </Accordion>
                    )}
                </div>
              ) : (
                  <div>loading....</div>
                )}
            </div>
          </div>
        </Drawer>
      </ThemeProvider>
    </React.Fragment>
  );
}
