using LinqKit;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Catalogue.Commands;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using Oscar.Infrastructure.Features.MandateTypes.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class CatalogueForm
    {
        private CatalogueDto? _catalogue;
        private String _name = "Loading...";
        private String _iMaestroClientCode = "";
        private String _agicoaClientRef = "";
        private string _generalNotes = "";
        private ICollection<MandateTypeDto> _mandateTypes { get; set; } = new List<MandateTypeDto>();
        public bool _showDetailsPanel = false;
        public bool _showSocietiesPanel = false;
        public bool _showRightsPanel = false;
        public bool _showOtherNamesPanel = false;
        public bool _showMandatesPanel = false;
        public bool _isUpdating = false;
        public ICollection<SocietyDto> _societies = new List<SocietyDto>();
        public ICollection<OtherNameDto> _otherNames = new List<OtherNameDto>();

        [Parameter]
        public EventCallback<CatalogueDto> ValueChanged { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        [Parameter]
        public int _clientId { get; set; }

        [Parameter]
        public int _catalogueId { get; set; }

        [Parameter]
        public EventCallback<bool> toggleDrawer { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            await LoadCatalogue();
            StateHasChanged();
        }


        private async Task LoadCatalogue()
        {
            if (_catalogueId > 0)
            {
                _catalogue = (await Mediator.Send(new GetCatalogueByIdQuery()
                {
                    Id = _catalogueId
                }
                )).Value;
                if (_catalogue != null)
                {
                    _name = _catalogue.Name;
                    _iMaestroClientCode = _catalogue.IMaestroClientCode;
                    _agicoaClientRef = _catalogue.AgicoaClientRef;
                    _generalNotes = _catalogue.GeneralNotes;
                    await LoadMandateTypes();
                }
                if (_catalogue != null && _catalogue.Societies != null && _catalogue.Societies.Count() > 0) _societies = _catalogue.Societies.Distinct().ToList();
                if (_catalogue != null && _catalogue.OtherNames != null && _catalogue.OtherNames.Count() > 0)
                {
                    _otherNames = _catalogue.OtherNames.ToList();
                }
                else { _otherNames = new List<OtherNameDto>(); }
            }
            else { _otherNames = new List<OtherNameDto>(); }

        }

        private async Task LoadMandateTypes()
        {
            _mandateTypes = (await Mediator.Send(new GetAllMandateTypesQuery())).Value.ToList();
            var mandates = _catalogue.Mandates.Where(x => x.Mandated).ToList();

            if (mandates != null && mandates.Any())
            {
                foreach (var mandate in mandates)
                {
                    var m = _mandateTypes.FirstOrDefault(x => x.Id == mandate.MandateType.Id);
                    if (m != null) { m.Mandated = true; }
                }
            }
            else //default to true when no mandates already exists
            {
                foreach (var mandateType in _mandateTypes)
                    mandateType.Mandated = true;
            }
        }

        private async Task UpdateCatalogue()
        {
            _isUpdating = true;

            var updateCatalogueCommand = new UpdateCatalogueCommand()
            {
                Id = _catalogueId,
                CatalogueUpdateDto = new CatalogueUpdateDto()
                {

                    Name = _name,
                    ClientId = _clientId,
                    IMaestroClientCode = _iMaestroClientCode,
                    AgicoaClientRef = _agicoaClientRef,
                    GeneralNotes = _generalNotes,
                    OtherNames = _otherNames,
                    MandateTypes = _mandateTypes,
                }
            };
            var result = await Mediator.Send(updateCatalogueCommand);
            if (result.IsSuccess)
            {
                Snackbar.Add("Catalogue successfully updated", Severity.Success);
                await SuccessCallback();
                resetForm();

            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }

            _isUpdating = false;

        }



        public void resetForm()
        {
            _showDetailsPanel = false;
            _showSocietiesPanel = false;
            _showRightsPanel = false;
            _showOtherNamesPanel = false;
            _showMandatesPanel = false;
            _name = "";
            _iMaestroClientCode = "";
            _agicoaClientRef = "";
            _generalNotes = "";
            _otherNames = new List<OtherNameDto>();
            StateHasChanged();

        }


        protected async Task SuccessRightsCallback()
        {
            //openCatalogFormDrawer = false;
            //await LoadCatalogues();
        }

        protected async Task SuccessCallback()
        {
            await onSuccess.InvokeAsync();
        }

        protected async Task Cancel()
        {
            await toggleDrawer.InvokeAsync(false);
        }

        private EventCallback showHideDetailsPanel => new(this, (Action<bool>)((bool isExpanded) => _showDetailsPanel = isExpanded));
        private EventCallback showHideSocitiesPanel => new(this, (Action<bool>)((bool isExpanded) => _showSocietiesPanel = isExpanded));
        private EventCallback showHideRightsPanel => new(this, (Action<bool>)((bool isExpanded) => _showRightsPanel = isExpanded));
        private EventCallback showHideOtherNamesPanel => new(this, (Action<bool>)((bool isExpanded) => _showOtherNamesPanel = isExpanded));
        private EventCallback showHideMandatesPanel => new(this, (Action<bool>)((bool isExpanded) => _showMandatesPanel = isExpanded));

    }
}
