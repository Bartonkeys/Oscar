using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Blazor.Library.Components.Clients;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class ClientAndCatPicker
    {
        private List<ClientBasicDto> _clients = new();
        private ClientBasicDto _client;
        private List<CatalogueDto> _catalogues = new();
        private CatalogueDto _catalogue;

        [Parameter]
        public List<int> ClientIds { get; set; }

        [Parameter]
        public ICollection<CatalogueDto> Catalogues { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        protected async Task ReloadCats()
        {
            _catalogues.Clear();
            if (ClientIds.Any())
            {
                ClientDto client = (await Mediator.Send(new GetClientByIdQuery
                {
                    Id = (int)ClientIds.First()
                })).Value;
                _catalogues = client.Catalogues.ToList();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            _clients = (await Mediator.Send(new GetClientBasicQuery())).Value;
            await ReloadCats();
        }

        private async Task<IEnumerable<ClientBasicDto>> SearchClients(string value)
        {
            return string.IsNullOrEmpty(value) ? _clients : _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async void AddClient()
        {
            if (ClientIds.Count() > 0)
            {
                Snackbar.Add("Only one Owning Client permitted. Please delete the current owning client before adding a new one.", Severity.Error);
                return;
            }

            if (_client != null && !ClientIds.Contains(_client.Id))
            {
                ClientIds.Add(_client.Id);
                await ReloadCats();
            }
        }

        private async void RemoveClient(int clientId)
        {
            ClientIds.Remove(clientId);
            Catalogues.Clear();
            _catalogue = null;
            await ReloadCats();
        }

        private async Task<IEnumerable<CatalogueDto>> SearchCatalogues(string value)
        {
            return string.IsNullOrEmpty(value) ? _catalogues : _catalogues.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async void AddCatalogue()
        {
            if (Catalogues.Count() > 0)
            {
                Snackbar.Add("Only one Owning Catalogue permitted. Please delete the current owning catalogue before adding a new one.", Severity.Error);
                return;
            }

            if (_catalogue != null && !Catalogues.Contains(_catalogue))
            {
                Catalogues.Add(_catalogue);
            }
        }

        private async void RemoveCatalogue(CatalogueDto catalogue)
        {
            Catalogues.Remove(catalogue);
        }
    }
}

