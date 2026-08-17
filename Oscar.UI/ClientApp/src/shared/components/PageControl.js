import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Button, MenuItem, TextField } from '@mui/material';
import { FirstPage, ChevronLeft, ChevronRight, LastPage } from '@mui/icons-material';
import { sleep } from '../helpers/jshelper';

let numberType = 0;

export default function PageControl(props) {
    const [pageDetails, setPageDetails] = useState({
        totalRecords: props.totalRecords,
        page: props.currentPage ? props.currentPage : 1,
        pageLength: props.pageLength,
        hasLoaded: false
    });

    const [typedNumber, setTypedNumber] = useState(pageDetails.page);

    const takeOptions = [
        20, 50, 100
    ];

    useEffect(() => {
        setPageDetails(p => { return { ...p, page: props.currentPage || 1 } });
        setTypedNumber(props.currentPage || 1);
    }, [props.currentPage]);

    useEffect(() => {
        setPageDetails(p => { return { ...p, totalRecords: props.totalRecords || 0 } })
    }, [props.totalRecords])

    useEffect(() => {
        if (pageDetails.hasLoaded) {
            let start = (pageDetails.page - 1) * pageDetails.pageLength;
            let take = pageDetails.pageLength;
            props.updatePage(start, take, pageDetails.page);
        }
        else {
            setPageDetails({ ...pageDetails, hasLoaded: true });
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [pageDetails.page, pageDetails.pageLength]);

    useEffect(() => {
        (async () => {
            let value = typedNumber;

            numberType++;
            await sleep(300);
            if (numberType === 1 && value !== pageDetails.page) {

                if (value < 1) {
                    setPageDetails({ ...pageDetails, page: 1 });
                }
                else if (value > pageDetails.totalRecords / pageDetails.pageLength) {
                    setPageDetails({ ...pageDetails, page: Math.ceil(pageDetails.totalRecords / pageDetails.pageLength) });
                }
                else {
                    setPageDetails({ ...pageDetails, page: value });
                }
            }
            numberType--;
        })();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [typedNumber]);

    function numberChange(e) {
        let value = +e.target.value;
        setTypedNumber(value);
    }

    function nextPage() {
        setPageDetails({ ...pageDetails, page: +pageDetails.page + 1 });
    }

    function firstPage() {
        setPageDetails({ ...pageDetails, page: 1 });
    }

    function previousPage() {
        setPageDetails({ ...pageDetails, page: +pageDetails.page - 1 });
    }

    function lastPage() {
        setPageDetails({ ...pageDetails, page: Math.ceil(pageDetails.totalRecords / pageDetails.pageLength) });
    }

    function setTake(e) {
        let page = pageDetails.page > pageDetails.totalRecords / e.target.value ? Math.ceil(pageDetails.totalRecords / e.target.value) : pageDetails.page;

        setPageDetails({ ...pageDetails, page: page, pageLength: e.target.value });
    }

    return (
        <div>
            <div className="flexRow flexMiddle anchor">
                {props.numberSelected ?
                    <div className="absolute absoluteLeft absoluteVCentre flexRow flexCentre">
                        <div>{props.numberSelected} Selected</div>
                        <div className="clearButton ml-2" onClick={() => props.clearSelected()}>Clear</div>
                    </div> : <div></div>}
                <div className="flexRow">
                    <Button size="small" variant="contained" disabled={pageDetails.page <= 1} onClick={firstPage}><FirstPage></FirstPage></Button>
                    <Button size="small" variant="contained" disabled={pageDetails.page <= 1} onClick={previousPage}><ChevronLeft></ChevronLeft></Button>
                    <div style={{ width: '70px' }}><TextField size="small" variant="outlined" type="number" value={pageDetails.page} onChange={numberChange}></TextField></div>
                    <Button size="small" variant="contained" disabled={pageDetails.page >= pageDetails.totalRecords / pageDetails.pageLength} onClick={nextPage}><ChevronRight></ChevronRight></Button>
                    <Button size="small" variant="contained" disabled={pageDetails.page >= pageDetails.totalRecords / pageDetails.pageLength} onClick={lastPage}><LastPage></LastPage></Button>
                </div>
                <div className="absolute absoluteRight absoluteVCentre flexRow flexCentre minorText"><div className="mr-1"></div><TextField size="small" select value={pageDetails.pageLength} onChange={setTake}> {takeOptions.map(x => (<MenuItem key={x} value={x}>{x} records</MenuItem>))} </TextField></div>
            </div>
            <div className="flexRow flexMiddle mt-1 minorText">
                Showing {(pageDetails.page - 1) * pageDetails.pageLength + 1} - {pageDetails.page * pageDetails.pageLength < pageDetails.totalRecords ? pageDetails.page * pageDetails.pageLength : pageDetails.totalRecords} of {pageDetails.totalRecords}
            </div>
        </div>
    );
}

PageControl.propTypes = {
    pageLength: PropTypes.number.isRequired,
    totalRecords: PropTypes.number.isRequired,
    updatePage: PropTypes.func.isRequired,
    currentPage: PropTypes.number,
    numberSelected: PropTypes.number,
    clearSelected: PropTypes.func
}