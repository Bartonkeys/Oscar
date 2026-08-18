using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Blazor.Pages
{
    public partial class RightsBulkAmend: OscarComponentBase
    {
        private ClientDto? _selectedClient;
        private CatalogueDto? _selectedCatalogue;
        private MudAutocomplete<ClientDto> clientSelect;
        private MudSelect<CatalogueDto> catalogueSelect;
        private IEnumerable<ClientDto> _clients;
        private bool _processing;

        private bool _rightsPerpetuity;
        private bool _rightsValidityPerpetuity;
        private DateTime? _endOfRight;
        private DateTime? _endOfValidity;
        private DateTime? _startOfRight;
        private DateTime? _startOfValidity;

        protected override async void OnInitialized()
        {
            await LoadClients();
        }

        private async Task LoadClients()
        {
            _clients = (await Mediator.Send(new GetAllClientsQuery())).Value.ToList();
        }

        private async Task<IEnumerable<ClientDto>> SearchClients(string value, CancellationToken token)
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
        }

        private async Task BulkAmendRights()
        {
            var confirmResult = await (await DialogService.ShowAsync<ConfirmDialog>("This process will update every relevant work associated with this client and catalogue")).Result;
            if (!confirmResult.Canceled)
            {
                _processing = true;
                StateHasChanged();

                var result = await Mediator.Send(new AmendBulkRightsCommand()
                {
                    ClientId = _selectedClient?.Id,
                    CatalogueId = _selectedCatalogue?.Id,
                    StartOfRight = _startOfRight,
                    StartOfValidity = _startOfValidity,
                    EndOfRight = !_rightsPerpetuity ? _endOfRight : new DateTime(9999, 12, 31),
                    EndOfValidity = !_rightsValidityPerpetuity ? _endOfValidity : new DateTime(9999, 12, 31)
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
        }

    }
}
