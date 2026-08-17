let workColumns = [
    {
        displayName: 'Type',
        jsonField: 'discriminator',
        sortable: false,
        width: '15%'
    },
    {
        displayName: 'Title',
        jsonField: 'titles',
        sortable: false,
        width: '40%'
    },
    {
        displayName: 'Duration (mins)',
        jsonField: 'durationMinutes',
        sortable: false,
        width: '15%',
        align: 'center'
    },
    {
        displayName: 'Production year',
        jsonField: 'productionYear',
        sortable: false,
        width: '15%',
        align: 'center'
    },
    {
        displayName: 'First Broadcast Year',
        jsonField: 'firstBroadcastYear',
        sortable: false,
        width: '15%',
        align: 'center'
    }
];

export default workColumns;