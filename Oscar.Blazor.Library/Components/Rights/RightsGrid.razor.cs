using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Severity = MudBlazor.Severity;
using FluentValidation;
using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Rights.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using Oscar.Infrastructure.Features.Episode.Queries;
using Oscar.Infrastructure.Features.Season.Queries;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class RightsGrid
    {
        private string _saveText = "Create Right";
        private ClientDto? selectedClient;
        private LightWeightWorksDto? selectedWork;
        private CatalogueDto? selectedCat;
        private MudTable<RightDto> table;
        private string _searchString = null;
        private List<RightDto> _allRights;
        private List<RightDto> _rights;
        private IEnumerable<CatalogueDto?> _catalogues;

        private bool _editMode = false;
        private RightDto _editRight = null;

        private IEnumerable<RightsTypeDto> _rightsType = new List<RightsTypeDto>();

        private MudSelect<RightsTypeDto>? typeSelect;
        private MudDatePicker? startDate;
        private MudDatePicker? endDate;
        private MudDatePicker? startValid;
        private MudDatePicker? endValid;
        private MudTextField<string>? notations;
        private MudNumericField<decimal> percentage;
        private MudButton createRight;

        private RightDto _selectedItem = new();
        private Discriminator _discriminator;
        private CatalogueDto? _selectedCatalogue;

        private bool _openRightsDrawer;
        private int _clientId { get; set; }
        private int _worksId { get; set; }
        private int _catalogueId { get; set; }
        private RightDto _right { get; set; }

        private bool _isInheritedRights;

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public bool Loading { get; set; } = true;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool IsSeason { get; set; } = false;

        [Parameter]
        public bool Works { get; set; } = false;

        [Parameter]
        public RightsSource RightsSource { get; set; }

        [Parameter]
        public int? WorksProductionYear { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        [Parameter]
        public int FilterClientID { get; set; }

        [Parameter]
        public int? FilterWorkID { get; set; }

        [Parameter]
        public int FilterCatID { get; set; }

        [Parameter]
        public bool DisableCatalogSelection { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Parameter]
        public bool SuppressCatalogueDropdown { get; set; } = false;

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IList<RightsTypeDto>? RightsType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadRightsTypes();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Open && FilterCatID > 0)
            {
                if (_catalogueId == FilterCatID) return;

                _catalogueId = FilterCatID;
                await LoadRights();
            }
            else if (Open & Id > 0 && Id != _currentId)
            {
                _currentId = Id;
                await LoadRights();
            }
        }

        private async Task EditRight(RightDto right)
        {
            _editMode = true;
            _editRight = right;

            if (right.Work == null)
            {
                FilterWorkID = null;
                selectedWork = null;
            }
            else
            {
                FilterWorkID = right.Work.Id;
            }

            typeSelect.SelectedValues = new List<RightsTypeDto> { right.Type };

            startDate.Date = right.StartOfRight;
            endDate.Date = right.EndOfRight;
            startValid.Date = right.StartOfValidity;

            if (right.EndOfValidity != null && right.EndOfValidity != DateTime.MaxValue)
                endValid.Date = right.EndOfValidity;

            await notations.SetTextAsync(right.Notations);

            // this is horrible, there has to be a way of forcing it to refresh it's value better than this
            percentage.Value = right.Percentage.Value;
            percentage.Text = right.Percentage.Value.ToString();
            percentage.Increment();
            percentage.Decrement();

            _saveText = "Save Changes";
        }

        private async Task DeleteRight(RightDto right)
        {
            if (right != null && right.Id > 0)
            {
                var deleteRightCommand = new DeleteRightCommand()
                {
                    RightDeleteDto = new RightDeleteDto
                    {
                        ID = right.Id
                    }
                };

                var resultRight = await Mediator.Send(deleteRightCommand);
                await HandleDeleteResult<RightDto>(resultRight);
            }
        }

        static string getRightsTypeString(RightsTypeDto input)
        {
            if (input == null)
                return string.Empty;

            string returnVal = input.Description;

            if (input.Name != null && input.Name != string.Empty)
                returnVal += " (" + input.Name + ")";

            return returnVal;
        }

        public Func<RightsTypeDto, string> converter = getRightsTypeString;
        private int _currentId;
        private string _inheritedFrom;
        private bool _isProcessingRights;
        private bool _isDeletingAtThisLevel;
        private bool _isDeletingAtAllLevels;
        private bool _isUpdateRightsFromHeader;

        public async void onChange(EventArgs args)
        {
            if (selectedClient != null)
                FilterClientID = selectedClient.Id;
            else
                FilterClientID = 0;
        }

        private async Task HandleDeleteResult<T>(Result<RightDto> result) where T : RightDto
        {
            if (result.IsSuccess)
            {
                await Task.Run(LoadRights);
                StateHasChanged();
                Snackbar.Add("Successfully removed Right", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task LoadRights()
        {
            await SetStatusAsync(true, "Loading Rights");
            try
            {
                Loading = true;
                _isInheritedRights = false;
                var result = Result.Fail<IEnumerable<RightDto>>("Not set");

                switch (RightsSource)
                {
                    case RightsSource.Client:
                        result = await PopulateClientRights();
                        break;
                    case RightsSource.Works:
                        result = await PopulateWorksRights();
                        break;
                    case RightsSource.Catalogue:
                        result = await PopulateCatalogueRights();
                        break;
                }

                await SetRightsCountryGroups();
                if (result.IsFailure) return;
            }
            finally
            {
                Loading = false;
                await SetStatusAsync(false,"Rights Loaded" );
            }
        }

        private async Task<List<CountriesGroupsDto>> PopulateCountriesGroups()
        {
            var countries = (await Mediator.Send(new GetAllCountriesQuery())).Value;

            var countriesGroups = new List<CountriesGroupsDto>();
            countriesGroups?.Clear();
            var world = countries.FirstOrDefault(x => x.Code == "*");
            if (world != null)
                countriesGroups?.Add(new CountriesGroupsDto { Id = world.Id, Code = world.Code, Name = world.Name });
            var canada = countries.FirstOrDefault(x => x.Code == "CA");
            if (canada != null)
                countriesGroups?.Add(new CountriesGroupsDto { Id = canada.Id, Code = canada.Code, Name = canada.Name });
            var uk = countries.FirstOrDefault(x => x.Code == "GB");
            if (uk != null)
                countriesGroups?.Add(new CountriesGroupsDto { Id = uk.Id, Code = uk.Code, Name = uk.Name });
            var usa = countries.FirstOrDefault(x => x.Code == "US");
            if (usa != null)
                countriesGroups?.Add(new CountriesGroupsDto { Id = usa.Id, Code = usa.Code, Name = usa.Name });

            //Get all countries under their respective groups and then add them as well
            var allCountriesGroups = (await Mediator.Send(new GetAllCountriesGroupsQuery())).Value;

            foreach (var group in allCountriesGroups)
            {
                if (group.HasChild)
                    countriesGroups?.Add(group);
            }

            countriesGroups?.AddRange(countries.Where(x => x.Code != "*" && x.Code != "CA" && x.Code != "GB" && x.Code != "US")
                .Select(x => new CountriesGroupsDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                }));

            return countriesGroups;
        }

        private async Task SetRightsCountryGroups()
        {
            foreach (var right in _rights)
            {
                right.CountriesGroups = await PopulateCountriesGroups();
                //for all associated countries under given right
                foreach (var country in right.Countries)
                {
                    SetSelectedCountriesChecked(country, right.CountriesGroups);
                }

                foreach (var item in right.CountriesGroups)
                {
                    //if group is checked then that implies all countries under this group are checked
                    if (item.HasChild && item.IsChecked)
                    {
                        right.CountryCountryGroup.Add(item.Name); //Add group name
                        right.CountriesWithinSelectedGroups.AddRange(item.Children.Select(x => x.Name));
                    }
                    else if (item.HasChild && !item.IsChecked) // when some but not all countries within a group is selected
                    {
                        right.CountryCountryGroup.AddRange(item.Children.Where(x => x.IsChecked).Select(x => x.Name));
                    }
                    else if (item.IsChecked)//countries which are not within any group
                    {
                        right.CountryCountryGroup.Add(item.Name); //Add Country
                    }
                }

                //remove repeating countries if they already are under selected groups to reduce display list
                right.CountryCountryGroup.RemoveAll(countryName =>
                            {
                                if (right.CountriesWithinSelectedGroups.Contains(countryName))
                                    return true;
                                else
                                    return false;
                            });
                right.CountryCountryGroup = right.CountryCountryGroup.Distinct().ToList();
            }
        }

        private void SetSelectedCountriesChecked(CountryDto country, ICollection<CountriesGroupsDto> countriesGroups)
        {
            if (countriesGroups != null)
            {
                foreach (var countriesGroup in countriesGroups)
                {
                    //match for countries at root level i.e individual countries without being inside group
                    //This could match with a CountryGroup hence make sure its a Country by matching with code as well
                    if (countriesGroup.Id == country.Id && countriesGroup.Code == country.Code)
                        countriesGroup.IsChecked = true;

                    //match for countries within a group
                    SetSelectedChildCountriesChecked(countriesGroup, country);
                }
            }
        }

        private void SetSelectedChildCountriesChecked(CountriesGroupsDto countriesGroup, CountryDto country)
        {
            if (countriesGroup.HasChild)
            {
                foreach (CountriesGroupsDto child in countriesGroup.Children)
                {
                    if (child.Id == country.Id && child.Code == country.Code)
                        child.IsChecked = true;
                }
                countriesGroup.IsChecked = countriesGroup.Children.All(i => i.IsChecked);
            }
        }

        private async Task LoadRightsTypes()
        {
            if (!_rightsType.Any())
            {
                _rightsType = await RefDataService.GetRightsType(); 
            }
            Loading = false;
        }

        private async Task<Result<IEnumerable<RightDto>>> PopulateClientRights()
        {
            var clientResult = (await Mediator.Send(new GetClientByIdQuery { Id = Id }));
            var result = (await Mediator.Send(new GetRightsByClientIdQuery() { ClientId = Id }));

            _allRights = _rights = result.Value.ToList();
            _catalogues = result.Value.Where(c => c.Catalogue != null).Select(c => c.Catalogue).DistinctBy(c => c.Name);
            _selectedCatalogue = _catalogues != null && _catalogues.Any() ? _catalogues.SingleOrDefault(c => c.Name == clientResult.Value.ClientName) ?? _catalogues.First() : null;
            _rights = _allRights.Where(r => r.Catalogue?.Id == _selectedCatalogue?.Id).ToList();

            return result;
        }

        private async Task<Result<IEnumerable<RightDto>>> PopulateWorksRights()
        {
            var result = await Mediator.Send(new GetRightsByWorksIdQuery { WorksId = Id, });
            var work = await Mediator.Send(new GetWorksByIdQuery { Id = Id });

            if (work.Value is { Catalogues: { } })
                _catalogues = work.Value.Catalogues.DistinctBy(c => c.Name);

            _discriminator = Enum.Parse<Discriminator>(work.Value.Discriminator!);

            //works will be allowed to attach to only 1 catalogue hence FirstOrDefault() should fits here
            _selectedCatalogue = _catalogues.FirstOrDefault();
            _allRights = _rights = result.Value.ToList();

            if (!_allRights.Any()) await InheritRights(work);

            var client = await Mediator.Send(new GetClientByWorkIdQuery { Id = Id });

            if (client.Value != null)
                _clientId = client.Value.Id;

            return result;
        }

        private async Task InheritRights(Result<WorksDto> work)
        {
            _isInheritedRights = true;

            var discriminator = Enum.Parse<Discriminator>(work.Value.Discriminator!);
            switch (discriminator)
            {
                case Discriminator.StandAlone:
                case Discriminator.Series:
                    _allRights = await InheritCatalogueRights();
                    _inheritedFrom = "CATALOGUE";
                    break;
                case Discriminator.Season:
                    _allRights = await InheritSeriesRights(discriminator);
                    _inheritedFrom = "SERIES";
                    if (!_allRights.Any())
                    {
                        _allRights = await InheritCatalogueRights();
                        _inheritedFrom = "CATALOGUE";
                    }
                    break;
                case Discriminator.Episode:
                    _allRights = await InheritSeasonRights();
                    _inheritedFrom = "SEASON";
                    if (!_allRights.Any())
                    {
                        _allRights = await InheritSeriesRights(discriminator);
                        _inheritedFrom = "SERIES";
                    }
                    if (!_allRights.Any())
                    {
                        _allRights = await InheritCatalogueRights();
                        _inheritedFrom = "CATALOGUE";
                    }
                    break;
            }

            if (_inheritedFrom == "CATALOGUE")
            {
                foreach (var right in _allRights.Where(right => work.Value.ProductionYear is > 0))
                {
                    right.StartOfRight = new DateTime(work.Value.ProductionYear!.Value, 1, 1);
                    right.StartOfValidity = new DateTime(work.Value.ProductionYear!.Value, 1, 1);
                }
            }

            _rights = _allRights;
        }

        private async Task<List<RightDto>> InheritSeriesRights(Discriminator discriminator)
        {
            int? parentWorksId = null;
            switch (discriminator)
            {
                case Discriminator.Season:
                    var seasonResult = await Mediator.Send(new GetSeasonBasicByIdQuery { Id = Id });
                    if (seasonResult.IsSuccess) parentWorksId = seasonResult.Value.Series?.Id;
                    break;
                case Discriminator.Episode:
                    var episodeResult = await Mediator.Send(new GetEpisodeByIdQuery() { Id = Id });
                    if (episodeResult.IsSuccess) parentWorksId = episodeResult.Value.Series?.Id;
                    break;
            }

            if (parentWorksId == null) return new List<RightDto>();

            var rightsResult = await Mediator.Send(new GetRightsByWorksIdQuery { WorksId = parentWorksId.Value });

            return rightsResult.IsSuccess ? rightsResult.Value.ToList() : new List<RightDto>();
        }

        private async Task<List<RightDto>> InheritSeasonRights()
        {
            int? parentWorksId = null;
            var episodeResult = await Mediator.Send(new GetEpisodeByIdQuery() { Id = Id });
            if (episodeResult.IsSuccess) parentWorksId = episodeResult!.Value.Season?.Id;

            if (parentWorksId == null) return new List<RightDto>();

            var rightsResult = await Mediator.Send(new GetRightsByWorksIdQuery { WorksId = parentWorksId.Value });

            return rightsResult.IsSuccess ? rightsResult.Value.ToList() : new List<RightDto>();
        }

        private async Task<List<RightDto>> InheritCatalogueRights()
        {
            if (_selectedCatalogue == null) return new List<RightDto>();

            var catalogueRightsResult = (await Mediator.Send(new GetCatalogueRightsQuery()
            {
                Id = _selectedCatalogue.Id,
            }));

            return catalogueRightsResult.IsSuccess ? catalogueRightsResult.Value.ToList() : new List<RightDto>();
        }

        private async Task<Result<IEnumerable<RightDto>>> PopulateCatalogueRights()
        {
            var clientResult = (await Mediator.Send(new GetClientByIdQuery { Id = this.Id }));
            var result = (await Mediator.Send(new GetRightsByClientIdQuery
            {
                ClientId = Id,
                CatalogueId = FilterCatID
            }));

            _allRights = _rights = result.Value.ToList();
            _catalogues = result.Value.Where(c => c.Catalogue != null).Select(c => c.Catalogue).DistinctBy(c => c.Name);
            _selectedCatalogue = _catalogues != null && _catalogues.Any() ? _catalogues.SingleOrDefault(c => c.Name == clientResult.Value.ClientName) ?? _catalogues.First() : null;
            _rights = _allRights.Where(r => r.Catalogue?.Id == _selectedCatalogue?.Id).ToList();

            return result;
        }

        private bool Filter(RightDto right) => FilterBySearchString(right, _searchString);

        private static bool FilterBySearchString(RightDto right, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(right.Type.Name)
                   || right.Type.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || (!string.IsNullOrWhiteSpace(right.Type.Description) && right.Type.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                   || right.StartOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.EndOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.StartOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || right.EndOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }
        private async Task LoadForClients(int id)
        {
            _rights = _allRights.Where(r => r.Catalogue?.Id == id).ToList();
            _selectedCatalogue = _catalogues.SingleOrDefault(c => c?.Id == id);
            await SetRightsCountryGroups();
        }

        protected async Task OpenRightsDrawer(RightDto? right)
        {
            if (Works)
            {
                _worksId = Id;
            }
            else
            {
                _clientId = Id;
            }
            _right = right;

            if (_selectedCatalogue != null)
                _catalogueId = _selectedCatalogue.Id;
            _openRightsDrawer = true;
            await Task.CompletedTask;
        }

        protected async Task DeleteAllRights()
        {
            _isDeletingAtThisLevel = true;
            foreach (var right in _rights)
                await DeleteRight(right);
            _isDeletingAtThisLevel = false;
        }

        protected async Task ToggleDrawer(bool open)
        {
            _openRightsDrawer = open;
            await Task.CompletedTask;
        }

        private async Task UpdateRightsFromHeader()
        {
            _isUpdateRightsFromHeader = true;
            var result = await Mediator.Send(new UpdateRightsFromHeaderCommand
            {
                Id = Id,
                Rights = _rights,
                Discriminator = _discriminator
            });
            await HandleResult(result, "Successfully implemented Update Rights From Header");
            _isUpdateRightsFromHeader = false;
        }

        private async Task DeleteAllInheritedRights()
        {
            _isDeletingAtAllLevels = true;
            var result = await Mediator.Send(new DeleteRightsFromHeaderCommand
            {
                Id = Id,
                Discriminator = _discriminator
            });
            _isDeletingAtAllLevels = false;
            await HandleResult(result, _discriminator == Discriminator.Series ? "Successfully deleted rights for seasons and episodes" : "Successfully deleted rights fo episodes");
        }

        private async Task UseCatalogueRights()
        {
            _isProcessingRights = true;
            _rights = new List<RightDto>();

            var result = await Mediator.Send(new UseCatalogueRightsCommand
            {
                Id = Id
            });

            await HandleResult(result, "Successfully implemented Catalogue Rights");

            _isProcessingRights = false;
        }

        private async Task HandleResult(Result result, string message)
        {
            if (result.IsSuccess)
            {
                await Task.Run(LoadRights);
                StateHasChanged();
                Snackbar.Add(message, Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }
    }
}