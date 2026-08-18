using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Blazor.Pages
{
    public partial class RegistrationCreate
    {
        private IEnumerable<WorksDto> _works;
        private string _searchString = string.Empty;
        private WorksDto _selectedItem;
        private HashSet<WorksDto> selectedItems = new HashSet<WorksDto>();
        private Discriminator _discriminator = Discriminator.All;

        private IEnumerable<WorksDto> pagedData;
        private IEnumerable<ClientBasicDto> _clients = new List<ClientBasicDto>();
        private IEnumerable<SocietyDto> _societies = new List<SocietyDto>();

        private ClientBasicDto _client;
        private CatalogueDto _catalogue;
        private SocietyDto _society;
        private bool _includeAlreadyRegistered;

        private async Task LoadClients()
        {
            _clients = (await Mediator.Send(new GetClientBasicQuery())).Value.ToList();
        }

        private async Task LoadSocieties()
        {
            _societies = (await Mediator.Send(new GetAllSocietiesQuery())).Value.ToList();
        }

        protected override void OnInitialized()
        {
            LoadClients();
            LoadSocieties();
        }

        public void onFilterClear(EventArgs args)
        {
            _client = null;
            _catalogue = null;
            _discriminator = Discriminator.All;
            _society = null;
        }

        private async Task<IEnumerable<ClientBasicDto>> SearchClients(string value, CancellationToken token)
        {
            return string.IsNullOrEmpty(value) ? _clients : _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<SocietyDto>> SearchSocieties(string value, CancellationToken token)
        {
            //TODO Bring in other societies once available
            return _societies.Where(s => s.Name == "AGICOA");
            //return string.IsNullOrEmpty(value) ? _societies 
                //: _societies.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) && (_client == null || x.Clients.Any(c => c.Id == _client.Id)));
        }

        public async void SubmitRegistration()
        {
            var addRegistrationBatchCommand = new AddRegistrationBatchCommand
            {
                SocietyId = _society.Id,
                ClientId = _client.Id,
                CatalogueId = _catalogue?.Id,
                IncludePreviouslyRegisteredWorks = _includeAlreadyRegistered
            };

            var result = await Mediator.Send(addRegistrationBatchCommand);

            if (result.IsSuccess)
            {
                var registrationBatchCreateDto = result.Value;
                Snackbar.Add($"Registration batch {registrationBatchCreateDto.BatchId}", Severity.Success);
                ResetForm();
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async void ResetForm()
        {
            onFilterClear(null);
        }
    }
}
