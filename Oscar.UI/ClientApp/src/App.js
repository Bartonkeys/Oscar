import React, { Component, useState } from 'react';
import { Routes, Switch, Route } from "react-router-dom";
import history from './shared/helpers/history';
import { Layout } from './components/Layout';
import HomePage from './pages/Home/HomePage';
import { FetchData } from './components/FetchData';
import ClientPage from './pages/Client/ClientPage';
import WorkPage from './pages/Work/Work/WorkPage';
import MatchPage from './pages/Match/MatchPage';
import SeriesPage from './pages/Work/Series/SeriesPage';
import SeriesDetails from './pages/Work/Series/SeriesDetails';
import SeasonPage from './pages/Work/Season/SeasonPage';
import SeasonDetails from './pages/Work/Season/SeasonDetails';
import EpisodePage from './pages/Work/Episode/EpisodePage';
import EpisodeDetails from './pages/Work/Episode/EpisodeDetails';
import StandalonePage from './pages/Work/Standalone/StandalonePage';
import StandaloneDetails from './pages/Work/Standalone/StandaloneDetails';
import PrimarySearchAppBar from './shared/PrimarybarComponent/PgmSrch';
import { ThemeProvider } from '@mui/styles';
import { createTheme } from '@mui/material/styles';
import { ToastContainer } from 'react-toastify';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import ProtectedRoute from './components/ProtectedRoute';
import 'react-toastify/dist/ReactToastify.css';
import "./App.css";
import './common.css';

const baseTheme = createTheme();

export function setErrorMessage(message) {
    errorSetter(message);

    setTimeout(() => errorSetter(''), 8000);
}

export function setSuccessMessage(message) {
    successSetter(message);

    setTimeout(() => successSetter(''), 4000);
}

let errorSetter;
let successSetter;

function App() {
    const [errorMessage, setErrorMessage] = useState('');
    errorSetter = (msg) => setErrorMessage(msg);
    const [successMessage, setSuccessMessage] = useState('');
    successSetter = (msg) => setSuccessMessage(msg);

    return (
        <ThemeProvider theme={baseTheme}>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
                <div className="App">
                    <ToastContainer newestOnTop={true} />
                    <PrimarySearchAppBar />
                    <Routes>
                        <Route path="/" element={<HomePage />} />
                        <Route path='/allclients' element={<ProtectedRoute><ClientPage /></ProtectedRoute>} />
                        <Route path='/allworks' element={<ProtectedRoute><WorkPage /></ProtectedRoute>} />
                        <Route path='/allmatches' element={<ProtectedRoute><MatchPage /></ProtectedRoute>} />
                        <Route path='/allseries' element={<ProtectedRoute><SeriesPage /></ProtectedRoute>} />
                        <Route path='/allseries/:id' element={<ProtectedRoute><SeriesDetails /></ProtectedRoute>} />
                        <Route path='/allseasons' element={<ProtectedRoute><SeasonPage /></ProtectedRoute>} />
                        <Route path='/allseasons/:id' element={<ProtectedRoute><SeasonDetails /></ProtectedRoute>} />
                        <Route path='/allepisodes' element={<ProtectedRoute><EpisodePage /></ProtectedRoute>} />
                        <Route path='/allepisodes/:id' element={<ProtectedRoute><EpisodeDetails /></ProtectedRoute>} />
                        <Route path="/allstandalone" element={<ProtectedRoute><StandalonePage /></ProtectedRoute>}/>
                        <Route path="/allstandalone/:id" element={<ProtectedRoute> <StandaloneDetails /></ProtectedRoute>}/>
                    </Routes>
                </div>
            </LocalizationProvider>
        </ThemeProvider>
    );
}
export default App;
