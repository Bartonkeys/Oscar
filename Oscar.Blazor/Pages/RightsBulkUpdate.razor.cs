using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class RightsBulkUpdate: OscarComponentBase
    {
        private ClientDto? _selectedClient;
        private CatalogueDto? _selectedCatalogue;
        private MudAutocomplete<ClientDto> clientSelect;
        private MudSelect<CatalogueDto> catalogueSelect;
        private bool _initializing;
        private IEnumerable<ClientDto> _clients;
        private List<RightDto> _rights;
        private bool _processing;

        protected override async void OnInitialized()
        {
            _initializing = true;
            await LoadClients();
            _initializing = false;
        }

        private async Task LoadClients()
        {
            _clients = (await Mediator.Send(new GetAllClientsQuery())).Value.ToList();
        }

        private async Task<IEnumerable<ClientDto>> SearchClients(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _selectedClient = null;
                return _clients;
            }

            var filteredClients = _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredClients;
        }

        private async Task ClientChange(ClientDto selectedClient)
        {
            await catalogueSelect.ResetAsync();
            _selectedClient = selectedClient;
            var defaultCatalogueId = _selectedClient?.Catalogues.Min(c => c.Id);
            _selectedCatalogue = _selectedClient?.Catalogues.First(c => c.Id == defaultCatalogueId);

            await GetRights();
        }

        private async Task BulkAmendRights()
        {
            var confirmResult = await DialogService.Show<ConfirmDialog>("This process will update every relevant work associated with this client and catalogue").Result;
            if (!confirmResult.Canceled)
            {
                _processing = true;
                StateHasChanged();

                var result = await Mediator.Send(new UpdateBulkRightsCommand()
                {
                    ClientId = _selectedClient?.Id,
                    CatalogueId = _selectedCatalogue?.Id,
                    Rights = _rights
                });

                _processing = false;
                StateHasChanged();

                if (result.IsSuccess)
                    Snackbar.Add("Rights updated", Severity.Success);
                else 
                    Snackbar.Add(result.Error, Severity.Error);
            }
        }

        private async Task CatalogueChange(CatalogueDto obj)
        {
            _selectedCatalogue = obj;
            await GetRights();
        }

        private async Task GetRights()
        {
            if (_selectedClient == null) return;

            _rights = (await Mediator.Send(new GetRightsByClientIdQuery
            {
                ClientId = _selectedClient.Id,
                CatalogueId = _selectedCatalogue?.Id
            })).Value.ToList();

            StateHasChanged();
        }

    }
}
