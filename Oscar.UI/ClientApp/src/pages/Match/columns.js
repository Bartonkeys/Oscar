let columns = [
    {
        displayName: 'Ref',
        jsonField: 'reference',
        sortable: true,
        width: '25%'
    },
    {
        displayName: 'Status',
        jsonField: 'status',
        sortable: true,
        width: '25%'
    },
    {
        displayName: 'Requested by',
        jsonField: 'requestedBy',
        sortable: true,
        width: '25%'
    },
    {
        displayName: 'Download link',
        jsonField: 'matchingResultPublicUrl',
        sortable: false,
        width: '25%'
    }
];

export default columns;