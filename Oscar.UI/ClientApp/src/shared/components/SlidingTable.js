import React, { useEffect, useRef, useState } from 'react';
import PropTypes from 'prop-types';
import './slidingTable.css';
import { Button, CircularProgress, FormControl, MenuItem, Select } from '@mui/material';
import { Close, KeyboardArrowDown, KeyboardArrowUp, Edit } from '@mui/icons-material';
import TableIcon from '../components/TableIcon/TableIcon';
import PageControl from './PageControl';
import CheckBox from '@mui/material/Checkbox';
import { objectCompare } from '../helpers/jshelper';
import { useIsAuthenticated } from "@azure/msal-react";
import { search } from "../helpers/apiaccess"
import { isArray,  } from 'lodash';

export default function SlidingTable(props) {
    const [tableRows, setTableRows] = useState([]);
    const [fetchOptions, setFetchOptions] = useState({
        start: 0,
        take: props.takeValue ? props.takeValue : 20
    });
    const [fetchingState, setFetchingState] = useState(false);
    const [error, setError] = useState(false);
    const [leftPosition, setLeftPosition] = useState(props.static ? "0%" : "100%");
    const [selected, setSelected] = useState(-1);
    const [totalRecords, setTotalRecords] = useState(0);
    const [tableOffset, setTableOffset] = useState(false);
    const [selectedRows, setSelectedRows] = useState(props.selectedRows);
    const [previousFilters, setPreviousFilters] = useState(props.searchFilters);
    const tableRef = useRef(null);
    const isAuthenticated = useIsAuthenticated();

    useEffect(() => {
        setSelectedRows(new Set(props.selectedRows));
    }, [props.selectedRows])

    useEffect(() => {
        (async () => {
            if (props.searchFilters && fetchingState) {
                try {
                    if (isAuthenticated) {
                        const data = await search(
                            props.searchUri,
                            { start: props.pageNumber, take: props.takeValue, ...fetchOptions},
                            props.searchByQueryString? null: props.searchFilters,
                            props.searchByQueryString? props.searchFilters: null,
                        );
                        setTableRows(data ? data.records : []);
                        setTotalRecords(data.totalRecords);
                        setError(false);
                    }

                } catch (err) {
                    setError(true);
                }
                setFetchingState(false);
            }
        })();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [fetchOptions]);

    useEffect(() => {
        if (tableRef) {
            setTableOffset(tableRef.current.offsetWidth > tableRef.current.clientWidth);
        }
        else {
            setTableOffset(false);
        }
    }, [tableRows]);

    useEffect(() => {
        if (!objectCompare(previousFilters, props.searchFilters)) {
            setTotalRecords(0);
            setTableRows([]);
        }
        setPreviousFilters(props.searchFilters);
        setFetchOptions({ ...fetchOptions, start: ((props.pageNumber || 1) - 1) * fetchOptions.take });
        setFetchingState(true);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [props.searchFilters])

    useEffect(() => {
        setLeftPosition(props.left ? props.left : 0 + 'px');
    }, [props.left])

    useEffect(() => {
        setSelected(props.selection);
    }, [props.selection]);

    function pageChanged(start, take, page) {
        setTableRows([]);
        if (props.setTakeValue) {
            props.setTakeValue(take);
        }
        setFetchOptions({ ...fetchOptions, start: start, take: take });
        setFetchingState(true);
        if (props.setPageNumber) {
            props.setPageNumber(page);
        }
    }

    function selectedRow(id, row) {
        setSelected(id);
        props.selectRow(id, row);
    }

    function closeAction() {
        setLeftPosition('100%');
        setTimeout(() => {
            props.closeAction();
        }, 500);
    }

    function getRowValue(row, col) {
        let value = '';
        if (col.jsonProperty) {
            if (row[col.jsonField]) {
                value = row[col.jsonField][col.jsonProperty];
            }
            if (col.jsAction) {
                value = col.jsAction(value, row, props.searchFilters, tableRows);
            }
        }
        else if (col.jsAction) {
            value = col.jsAction(row[col.jsonField], row, props.searchFilters, tableRows);
        }
        else {
            value = row[col.jsonField];
            if (isArray(row[col.jsonField])) {
                if (row[col.jsonField][0]) {
                    value = <FormControl size="small">
                        <Select displayEmpty value={row[col.jsonField][0].title}>
                            {row[col.jsonField].map(({ title }) => {
                                return <MenuItem value={title} key={title}>{title}</MenuItem>
                            })}
                        </Select>
                    </FormControl>
                }
                else {
                    value = 'no title';
                }

            }
            else if (value && isValidUrl(value)) {
                value = <FormControl size="small">
                    <a href={value}> link </a>
                </FormControl>
            }
            
        }
        return value;
    }

    function getRowTitle(row, col) {
        let title = getRowValue(row, col);

        return typeof title === 'object' ? '' : title;
    }

    function isRowSelected(id) {
        return selectedRows.has(id);
    }

    function checkSelectedRow(id) {
        let newSet = new Set(selectedRows.keys());
        if (isRowSelected(id)) {
            newSet.delete(id);
        }
        else {
            newSet.add(id);
        }
        setSelectedRows(newSet)
        props.selectedRowsChanged([...newSet.keys()]);
    }

    function toggleSelectAll() {
        let currentIds = tableRows.map(row => row.id);
        let newSet = new Set();
        if (anySelected()) {
            newSet = new Set([...selectedRows.keys()].filter(x => !currentIds.includes(x)));
            setSelectedRows(newSet);
        }
        else {
            newSet = new Set([...selectedRows.keys()].concat(currentIds));
            setSelectedRows(newSet);
        }
        props.selectedRowsChanged([...newSet.keys()]);
    }

    function anySelected() {
        return tableRows.map(row => row.id).some(x => selectedRows.has(x))
    }

    function clearAllSelectedRows() {
        setSelectedRows(new Set());
        props.selectedRowsChanged([]);
    }

    function setSortColumn(column) {
        if (column) {
            let sortDirection = 'asc';
            if (column === fetchOptions.sortColumn && fetchOptions.sortDirection === 'asc') {
                sortDirection = 'desc';
            }
            setTableRows([]);
            setFetchOptions({ ...fetchOptions, sortColumn: column, sortDirection: sortDirection });            
            setFetchingState(true);
        }
    }

    const headers =
        <div>
            <div className="titleRow">
                {props.canSelectRows ? <div className="selectButton" key="selectAll"><CheckBox color="primary" size="small" checked={anySelected()} onChange={toggleSelectAll} /></div> : <div></div>}
                {props.columns.filter(x => !x.restricted || !x.restricted()).map(x =>
                    <div key={x.displayName} className={"flexRow titleCell" + (x.sortable ? " clickable" : "")} style={{ width: x.width }} onClick={() => setSortColumn(x.sortable ? x.jsonField : '')}>
                        <div>{x.displayName}</div>
                        {x.sortable ? (x.jsonField === fetchOptions.sortColumn ? <div className="sortIcons"> {fetchOptions.sortDirection === 'asc' ? <KeyboardArrowUp fontSize="small" /> : <KeyboardArrowDown fontSize="small" />}</div> :
                            <div className="sortIcons">  <KeyboardArrowUp fontSize="small" /> <KeyboardArrowDown fontSize="small" /> </div>) : <div></div>}
                    </div>
                )}
            </div>
        </div>;
    const body = <div className={tableOffset ? "tableBody scrollBar" : "tableBody"} ref={tableRef}>
        {tableRows.map(row => <div key={row.id} className={(row.id === selected ? "selected " : "") + "tableRow border"}>
            {props.canSelectRows ? <div className="selectButton" key={row.id}><CheckBox color="primary" size="small" checked={isRowSelected(row.id)} onChange={(e) => checkSelectedRow(row.id)} /></div> : <div></div>}
            <div className="tableRow" onClick={() => selectedRow(row.id, row)}>{props.columns.filter(x => !x.restricted || !x.restricted()).map(col =>
                <div style={{ width: col.width, textAlign: col.align }} title={getRowTitle(row, col)} className="bodyCell" key={row.id + '-' + col.displayName}>{getRowValue(row, col)}</div>
            )}
            {props.onEdit && <div className="flexRow flexMiddle"><TableIcon title="Details" clickAction={(e) => { e.stopPropagation(); props.onEdit(row.id); }}><Edit /></TableIcon></div>}
            {props.onEditWholeRow && <div className="flexRow flexMiddle"><TableIcon title="Details" clickAction={(e) => { e.stopPropagation(); props.onEditWholeRow(row); }}><Edit /></TableIcon></div>}
            </div>
        </div>)}
        {error && <div>Error loading table...</div>}
        {!error && fetchingState ? <div className="loaderIcon"><CircularProgress size={40} /></div> : <div></div>}
        {!error && !fetchingState && tableRows.length === 0 ? <div className="flexCol flexCentre flexMiddle h-100"><h2>No results found</h2><div className="minorText">Try a less restrictive search</div></div> : <div></div>}
    </div>

    return (
        <div className="raisedContainer movingPanel" style={{ left: leftPosition, width: 'calc(100% - ' + ((leftPosition === '0px' ? 0 : props.left) + 30) + 'px)' }}>
            <div className="flexCol h-100">
                <div className="flexRow">
                    <h3>{props.title}</h3>
                    {props.closeAction ? <div onClick={() => closeAction()} className="moveRight"><Button size="small" variant="contained" color="secondary"><Close /></Button></div> : <div></div>}
                </div>
                {props.children}
                <div className="tableArea">
                    {headers}
                    {body}
                </div>
                {totalRecords > 0 ? <PageControl currentPage={props.pageNumber} clearSelected={clearAllSelectedRows} numberSelected={selectedRows.size} totalRecords={totalRecords} pageLength={fetchOptions.take} updatePage={pageChanged}></PageControl> : <div></div>}
            </div>
        </div>
    );
}

const isValidUrl = urlString => {
    try {
        return Boolean(new URL(urlString));
    }
    catch (e) {
        return false;
    }
}

SlidingTable.propTypes = {
    title: PropTypes.string.isRequired,
    columns: PropTypes.arrayOf(PropTypes.shape({
        displayName: PropTypes.string.isRequired,
        jsonField: PropTypes.string.isRequired,
        width: PropTypes.string.isRequired
    })),
    searchFilters: PropTypes.object,
    searchUri: PropTypes.string.isRequired,
    selectRow: PropTypes.func.isRequired,
    closeAction: PropTypes.func,
    left: PropTypes.number,
    selection: PropTypes.number,
    takeValue: PropTypes.number,
    setTakeValue: PropTypes.func,
    canSelectRows: PropTypes.bool,
    selectedRowsChanged: PropTypes.func,
    selectedRows: PropTypes.array,
    static: PropTypes.bool,
    pageNumber: PropTypes.number,
    setPageNumber: PropTypes.func,
    onEdit: PropTypes.func,
    onEditWholeRow: PropTypes.func,
}