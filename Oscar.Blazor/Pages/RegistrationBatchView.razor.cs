using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Registration.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Blazor.Pages
{
    public partial class RegistrationBatchView
    {
        private bool _loading = true;
        private bool openRegistrationCreate;

        private IEnumerable<RegistrationBatchDisplayDto> pagedData;
        private MudTable<RegistrationBatchDisplayDto> table;

        private int totalItems;
        private string searchString = null;
        private List<ClientBasicDto> _clients = new List<ClientBasicDto>();
        private List<SocietyDto> _societies = new List<SocietyDto>();

        [Inject]
        ISnackbar Snackbar { get; set; }

        private async Task<TableData<RegistrationBatchDisplayDto>> ServerReload(TableState state, CancellationToken token)
        {
            table.Loading = true;
            var tableData = (await Mediator.Send(new GetRegistrationBatchQuery
            {
                Start = state.Page * state.PageSize,
                Take = state.PageSize
            })).Value;

            totalItems = tableData.TotalRecords;
            pagedData = tableData.Records.ToArray();
            _loading = false;
            return new TableData<RegistrationBatchDisplayDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        private void OpenRegistrationCreate()
        {
            openRegistrationCreate = true;
        }

        private async Task Refresh()
        {
            await table.ReloadServerData();
            openRegistrationCreate = false;

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

        private Color GetColour(RegisterStatus? contextStatus)
        {
            switch (contextStatus)
            {
                case RegisterStatus.Batch_Complete:
                    return Color.Success;
                case RegisterStatus.Batch_Export_Failed:
                case RegisterStatus.Errors_Within_Batch:
                    return Color.Error;
                case RegisterStatus.Scheduled:
                    return Color.Secondary;
                case RegisterStatus.Processing:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
