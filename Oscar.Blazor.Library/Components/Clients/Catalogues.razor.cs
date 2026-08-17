using FluentValidation;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Oscar.Blazor.Library.Components.Works;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Catalogue.Commands;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using Oscar.Infrastructure.Features.Works.Commands;
using static Oscar.Blazor.Library.Components.CopyForm;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class Catalogues
    {
        private List<CatalogueDto>? _catalogues;
        private CatalogueDto? _catalogue;
        private int _catalogueId;
        private String _name;
        private String _reference = Guid.NewGuid().ToString();
        private String _iMaestroClientCode = "";
        public bool _showCreatePanel = false;

        private bool openCatalogFormDrawer;

        [Parameter]
        public EventCallback onSuccess { get; set; }

        [Parameter]
        public int _clientId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadCatalogues();
            StateHasChanged();
        }
        protected override async Task OnParametersSetAsync()
        {
            await LoadCatalogues();
            StateHasChanged();
        }


        private async Task LoadCatalogues()
        {
            var catalogues = (await Mediator.Send(new GetCataloguesQuery()
            {
                ClientID = _clientId
            }
            )).Value.OrderBy(x => x.Name).ToList();

            _catalogues = catalogues;
        }


        private async Task RemoveCatalogue(CatalogueDto catalogue)
        {
            var confirmResult = await DialogService.Show<ConfirmDialog>("Are you sure you wish to permanently delete this Catalogue?").Result;
            if (!confirmResult.Cancelled)
            {
                var deleteCatalogueCommand = new DeleteCatalogueCommand()
                {
                    Id = catalogue.Id
                };
                var result = await Mediator.Send(deleteCatalogueCommand);
                if (result.IsSuccess)
                {
                    _catalogues.Remove(catalogue);
                    resetForm();
                    Snackbar.Add("Catalogue successfully deleted", Severity.Success);
                }
                else
                {
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                }
            }
            
        }

        private async Task OpenCatalogForm(int id)
        {
            openCatalogFormDrawer = true;
            _catalogueId = id;
            await Task.CompletedTask;
        }


        private async Task CreateCatalogue()
        {
            var addCatalogueCommand = new AddCatalogueCommand()
            {
                catalogueAddDto = new CatalogueAddDto()
                {
                    Name = _name,
                    ClientId = _clientId,
                    IMaestroClientCode = _iMaestroClientCode 
                }
            };
            var result = await Mediator.Send(addCatalogueCommand);
            if (result.IsSuccess)
            {
                _catalogues.Add(new CatalogueDto
                {
                    Id = result.Value.Id,
                    Client = result.Value.Client,
                    Name = result.Value.Name
                });
                Snackbar.Add("Catalogue successfully created", Severity.Success);
                await OpenCatalogForm(result.Value.Id);
                resetForm();
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private void LoadCatalogueLinkedWorks(CatalogueDto catalogue)
        {
            if (catalogue != null)
            {
                NavigationManager.NavigateTo("works" + "/" + _clientId + "/" + catalogue.Id);

            }
        }

        protected async Task SuccessCatalogueFormCallback()
        {
            openCatalogFormDrawer = false;
            await LoadCatalogues();
        }

        public void resetForm()
        {
            _showCreatePanel = false;
            _name = "";
            _reference = Guid.NewGuid().ToString();
            _iMaestroClientCode = "";
            StateHasChanged();

        }

        private EventCallback showHideCreateForm => new(this, (Action<bool>)((bool isExpanded) => _showCreatePanel = isExpanded));

        protected async Task ToggleDrawer(bool open)
        {
            openCatalogFormDrawer = open;

        }
    }
}
