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
using Oscar.Infrastructure.Features.Equivalence.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class Equivalence
    {
        private bool _loading = true;
        private bool openEquivalenceCreate;

        private IEnumerable<EquivalenceRequestDto> pagedData;
        private MudTable<EquivalenceRequestDto> table;

        private int totalItems;
        private string searchString = null;
        private List<ClientBasicDto> _clients = new List<ClientBasicDto>();
        private List<SocietyDto> _societies = new List<SocietyDto>();

        [Inject]
        ISnackbar Snackbar { get; set; }

        private async Task<TableData<EquivalenceRequestDto>> ServerReload(TableState state, CancellationToken token)
        {
            table.Loading = true;
            var tableData = (await Mediator.Send(new GetEquivalenceRequestsQuery
            {
                Start = state.Page * state.PageSize,
                Take = state.PageSize
            })).Value;

            totalItems = tableData.TotalRecords;
            pagedData = tableData.Records.ToArray();
            _loading = false;
            return new TableData<EquivalenceRequestDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        private void OpenEquivalenceCreate()
        {
            openEquivalenceCreate = true;
        }

        private async Task Refresh()
        {
            await table.ReloadServerData();
            openEquivalenceCreate = false;

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

        private Color GetColour(EquivalenceRequestStatus? contextStatus)
        {
            switch (contextStatus)
            {
                case EquivalenceRequestStatus.Processed:
                    return Color.Success;
                case EquivalenceRequestStatus.Failed:
                    return Color.Error;
                case EquivalenceRequestStatus.Scheduled:
                    return Color.Info;
                case EquivalenceRequestStatus.Processing:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
