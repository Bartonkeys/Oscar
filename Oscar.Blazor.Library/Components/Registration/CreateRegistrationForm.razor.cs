using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Registration.Commands;
using Oscar.Infrastructure.Features.Registration.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Registration
{
    public partial class CreateRegistrationForm
    {
        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public List<SocietyDto> Societies { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public string? Title { get; set; } = string.Empty;

        private IEnumerable<LightWeightWorksDto> _works;

        private bool _processing;
        private bool _default;

        private IEnumerable<ClientBasicDto> _clients = new List<ClientBasicDto>();
        private IEnumerable<SocietyDto> _societies = new List<SocietyDto>();
        private Discriminator _discriminator = Discriminator.All;

        private bool _includeAlreadyRegistered;
        private DateTime? _fromPreviousRegistration = DateTime.UtcNow.AddDays(-1);
        private DateTime? _toPreviousRegistration = DateTime.UtcNow.AddDays(1);

        private ClientBasicDto _client;
        private CatalogueDto _catalogue;
        private SocietyDto _society;
        private bool _allClients;
        private bool _doNotRegister = true;
        private bool _includeEpisodes = true;
        private MudTable<WorksDto> _table;
        private bool _loading;
        private HashSet<WorksDto> _selectedItems = new HashSet<WorksDto>();
        private IEnumerable<WorksDto> _pagedData;
        private int _totalItems;
        private HashSet<RegistrationWorksDto> _registrationWorks;
        private HashSet<RegistrationWorksDto> _selectedRegistrationItems = new();
        private MudAutocomplete<ClientBasicDto> _clientAutoComplete;

        protected override async Task OnParametersSetAsync()
        {
            _clients = Clients;
            _societies = Societies;
            await onFilterClear(null);
        }

        public async Task onFilterClear(EventArgs args)
        {
            _client = null;
            _catalogue = null;
            _discriminator = Discriminator.All;
            _society = null;
            _registrationWorks = new();
            _selectedRegistrationItems = new();
            if(_clientAutoComplete != null)
                await _clientAutoComplete.ResetAsync();
            StateHasChanged();
        }


        private async Task<IEnumerable<ClientBasicDto>> SearchClients(string value, CancellationToken token)
        {
            return string.IsNullOrEmpty(value) ? _clients : _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<SocietyDto>> SearchSocieties(string value, CancellationToken token)
        {
            //TODO Bring in other societies once available
            return _societies.Where(s => s.Name is "AGICOA" or "SUISSIMAGE" or "SCREENRIGHTS" or "CCC" or "CMC" or "CRC" or "EGEDA" or "GWFF" or "MPA" or "UPFAR ARGOA" or "MPLC");
            //return string.IsNullOrEmpty(value) ? _societies 
            //: _societies.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) && (_client == null || x.Clients.Any(c => c.Id == _client.Id)));
        }

        public async void SubmitRegistration()
        {
            if (_allClients == false)
            {
                if (_client == null || _client?.Id == 0)
                {
                    Snackbar.Add("'Client' must not be empty.", Severity.Error);
                    return;
                }
            }

            if (_society == null || _society?.Id == 0)
            {
                Snackbar.Add("'Society' must not be empty.", Severity.Error);
                return;
            }

            _processing = true;
            StateHasChanged();

            var addRegistrationBatchCommand = new AddRegistrationBatchCommand
            {
                SocietyId = _society.Id,
                ClientId = _allClients ? null : _client.Id,
                CatalogueId = _catalogue?.Id,
                IncludePreviouslyRegisteredWorks = _includeAlreadyRegistered,
                IsAllClients = _allClients,
                DoNotRegister = _doNotRegister,
                UserSelectedWorkIds = _selectedRegistrationItems.Select(r => r.Id)
            };

            var result = await Mediator.Send(addRegistrationBatchCommand);

            if (result.IsSuccess)
            {
                var registrationBatchCreateDto = result.Value;
                Snackbar.Add($"Registration batch {registrationBatchCreateDto.BatchId}", Severity.Success);
                await OnSubmit.InvokeAsync();
                await ResetForm();
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }

            _processing = false;
            StateHasChanged();
        }

        private async Task ResetForm()
        {
            await onFilterClear(null);
        }

        private async Task ShowAll()
        {
            await Task.Delay(1);

            _registrationWorks = null;
            _loading = true;
            StateHasChanged();

            _registrationWorks = (await Mediator.Send(new GetRegistrationWorksQuery()
            {
                ClientId = _client?.Id,
                CatalogueId = _catalogue?.Id,
                SocietyId = _society?.Id,
                PreviouslyRegisteredFlag = _includeAlreadyRegistered,
                FromPreviousRegistration = _fromPreviousRegistration,
                ToPreviousRegistration= _toPreviousRegistration,
                IncludeEpisodes = _includeEpisodes
            })).Value;

            _loading = false;
            StateHasChanged();
        }

    }
}
