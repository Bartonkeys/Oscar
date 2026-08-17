using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.Screenrights.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class Screenrights
    {
        private bool _loading = true;
        private bool openScreenrightsCreate;

        private IEnumerable<ScreenrightsRequestDto> pagedData;
        private MudTable<ScreenrightsRequestDto> table;

        private int totalItems;
        private string searchString = null;
        private List<ClientBasicDto> _clients = new List<ClientBasicDto>();
        private List<SocietyDto> _societies = new List<SocietyDto>();

        [Inject]
        ISnackbar Snackbar { get; set; }

        private async Task<TableData<ScreenrightsRequestDto>> ServerReload(TableState state)
        {
            table.Loading = true;
            var tableData = (await Mediator.Send(new GetScreenrightsRequestsQuery
            {
                Start = state.Page * state.PageSize,
                Take = state.PageSize
            })).Value;

            totalItems = tableData.TotalRecords;
            pagedData = tableData.Records.ToArray();
            _loading = false;
            return new TableData<ScreenrightsRequestDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        private void OpenScreenrightsCreate()
        {
            openScreenrightsCreate = true;
        }

        private async Task Refresh()
        {
            await table.ReloadServerData();
            openScreenrightsCreate = false;

        }

        protected override void OnInitialized()
        {
            LoadClients();
            LoadSocieties();
        }

        private async Task LoadClients()
        {
            _clients = (await Mediator.Send(new GetClientBasicQuery())).Value.ToList();
        }

        private async Task LoadSocieties()
        {
            _societies = (await Mediator.Send(new GetAllSocietiesQuery())).Value.ToList();
        }

        private Color GetColour(ScreenrightsRequestStatus? contextStatus)
        {
            switch (contextStatus)
            {
                case ScreenrightsRequestStatus.Processed:
                    return Color.Success;
                case ScreenrightsRequestStatus.Failed:
                    return Color.Error;
                case ScreenrightsRequestStatus.Scheduled:
                    return Color.Info;
                case ScreenrightsRequestStatus.Processing:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
