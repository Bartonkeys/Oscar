using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Components.Works;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Blazor.Components;
using static Oscar.Blazor.Library.Components.CopyForm;
using Microsoft.AspNetCore.Components.Web;
using WorksStatus = Oscar.Core.Enums.WorksStatus;
using Azure.Core;
using Oscar.Core.Entities;
using Microsoft.VisualBasic;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming
namespace Oscar.Blazor.Pages
{
    public partial class Works : OscarComponentBase
    {
        #region - DI -
        [Inject] private IBrowserDownload BrowserDownload { get; set; }

        [Inject] private ProtectedLocalStorage ProtectedLocalStore { get; set; }

        #endregion

        #region - Parameters -
        [Parameter] public int FilterClientID { get; set; }

        [Parameter] public int FilterCatalogueID { get; set; }
        #endregion

        #region - Private Declarations -
        protected EventConsole Console { get; set; }
        private Discriminator _discriminator = Discriminator.All;
        private string _searchDiscriminators = string.Empty;
        private PersonType _searchPerson = PersonType.Director;
        private WorksStatus _statusDiscriminator = WorksStatus.Any;
        private readonly int maxNumberOfRecordsForCsvExport = 10000;
        private bool _updatingPageSize = false;
        private bool _updatingSelectedColumns = false;

        private int? _worksTypeId = null;
        public int FilterCountryID { get; set; }
        private bool _userSearch = false;
        private static bool _showInfoBanner = true;
        private bool openWorksDrawer;
        private bool openCopyDrawer;
        private bool openRightsDrawer;
        private int worksId;
        private string actionText = "";
        private readonly int _breakSize = 2; // default column size, lower the number higher the columns
        private bool _fixedHeader = true;
        private bool _fixedFooter = true;
        private IEnumerable<WorksDto> _pagedData;
        IEntityTable<WorksDto> _worksTable;
        private TableData<WorksDto> _worksData;
        private HashSet<WorksDto> selectedItems = new();
        private MudTaMudGridble<WorksDto> _grid;
        private MudSelect<WorksListColumn?> _worksListColumnSelect;

        private ClientDto? _selectedClient;
        private CatalogueDto? selectedCatalogue;
        private CountryDto? _selectedCountry;
        private CountryDto? _rightsCountryValue;
        private CompanyDto? _productionCompany;

        private int totalItems;
        private string searchString = null;
        private string searchStringDF = null;
        private string searchStringDS = null;
        private string searchStringAF = null;
        private string searchStringAS = null;
        private string searchStringPF = null;
        private string searchStringPS = null;
        private string searchStringSF = null;
        private string searchStringSS = null;
        private string searchStringFirstName = null;
        private string searchStringSurname = null;
        private string searchStringAgicoaRef = null;
        private string searchStringCompactRef = null;
        private string searchStringAS400 = null;
        private int? productionYear = null;
        private string _sortColumn = null;
        private MudAutocomplete<ClientDto>? clientSelect;

        private SearchType _searchTypeDiscriminator = SearchType.StartsWith;

        //private bool _initializing = true;
        //private bool _clearing;
        private MudAutocomplete<WorksTitleResponseDto> _autoComplete;
        private bool _autoCompleteSearch;
        private WorksTitleResponseDto? _selectedWorks;
        private bool _hasNoRights;
        private WorksTypePicker _worksTypePicker;
        private MudSwitch<bool> _noRightsSwitch;
        private MudSwitch<bool> _alternateTitlesSwitch;
        private MudDatePicker _createFromPicker;
        private DateTime? _createFromDate;
        private DateTime? _createToDate;
        private MudDatePicker _createToPicker;
        private MudAutocomplete<CountryDto> _rightsCountryAutoComplete;
        private MudAutocomplete<CountryDto> _countryAutoComplete;
        private MudAutocomplete<CompanyDto> _productionCompanyAutoComplete;
        private IEnumerable<WorksListColumn?> columnsVisible = new HashSet<WorksListColumn?>();
        private MudExpansionPanel _advancedSearchPanel;
        private MudSelect<string> _discriminatorSelector = new();
        private SearchWorksQuery _searchWorksQuery = new();
        private bool _useStoredQuery;
        #endregion

        #region - Predicates -
        private static bool _mainTitlesPredicate(WorksTitleDto dto) => dto.TitleType.In(new[] { TitleType.Main, TitleType.Episode });
        private static bool _alternateTitlesPredicate(WorksTitleDto dto) => dto.TitleType.In(new[] { TitleType.MainAlternative, TitleType.EpisodeAlternative });
        #endregion

        #region - Constants -

        private const string C_Enter_Key = "Enter";
        private const string C_NumpadEnter_Key = "NumpadEnter";
        #endregion

        #region - Clients -

        protected ClientDto? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                if (_selectedClient != null)
                {
                    RefDataService.GetClientCatalogs(_selectedClient.Id,
                        results => { _selectedClient.Catalogues = results.ToCollection(); });
                }
            }
        }

        protected async Task<IEnumerable<ClientDto>> ClientSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                FilterClientID = 0;
                SelectedClient = null;
                return RefDataService.Empty<ClientDto>();
            }

            return await RefDataService.SearchClients(null, searchTerm);
        }

        private string ClientDisplayFormat(ClientDto? client)
        {
            return client != null ? $"{client.ClientName} ({client.Status})" : "";
        }

        #endregion

        #region - Countries -

        protected CountryDto? SelectedCountry
        {
            get => _selectedCountry;
            set => _selectedCountry = value;
        }

        protected CountryDto? SelectedRightsCountry
        {
            get => _rightsCountryValue;
            set => _rightsCountryValue = value;
        }

        private async Task<IEnumerable<CountryDto>> CountrySearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                FilterCountryID = 0;
                SelectedRightsCountry = null;
                return RefDataService.Empty<CountryDto>();
            }

            return await RefDataService.SearchCountries(searchTerm);
        }

        private string CountryDisplayFormat(CountryDto? item)
        {
            return item != null ? $"{item.Name} ({item.Code})" : "";
        }

        #endregion

        #region - Companies -

        private async Task<IEnumerable<CompanyDto>> CompanySearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _productionCompany = null;
                return RefDataService.Empty<CompanyDto>();
            }

            return await RefDataService.SearchCompanies(null, searchTerm);
        }

        private string CompanyDisplayFormat(CompanyDto? item)
        {
            return item != null ? item.Name : "";
        }

        #endregion

        #region - Titles Search -

        private async Task<IEnumerable<WorksTitleResponseDto>> TitlesSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _selectedWorks = null;
                return RefDataService.Empty<WorksTitleResponseDto>();
            }

            return await RefDataService.SearchTitles(searchTerm);
        }

        private async Task GetWorks(WorksTitleResponseDto? worksTitle)
        {
            if (worksTitle == null) return;
            _autoCompleteSearch = true;
            _selectedWorks = worksTitle;
            await _grid.ReloadServerData();
        }

        private async Task<IEntityTable<WorksDto>> GetAutoCompleteSearchResults()
        {
            if (_selectedWorks == null)
            {
                return EntityTable<WorksDto>.Create(new List<WorksDto> { }).WithTotal(0);
            }

            var works = (await Mediator.Send(new GetWorksByIdQuery { Id = _selectedWorks.WorksId })).Value;
            _autoCompleteSearch = false;
            return EntityTable<WorksDto>.Create(new List<WorksDto> { works }).WithTotal(1);
        }

        #endregion

        protected override void OnInitialized()
        {
            // Placeholder
            base.OnInitialized();
            InitSearchWorksQuery();
        }

        private async void InitSearchWorksQuery()
        {
            try
            {
                var resultPageSession = await ProtectedLocalStore.GetAsync<int>("rowsPerPage");
                var searchResult = await ProtectedLocalStore.GetAsync<SearchWorksQuery>("SearchWorksQuery");
                if (searchResult.Success && searchResult.Value != null)
                {
                    _searchWorksQuery = searchResult.Value;
                    if (FilterClientID > 0 ) //&& _searchWorksQuery.ClientID == null)
                    {
                        _searchWorksQuery.ClientID = FilterClientID;
                    }
                    else
                    {
                        searchString = _searchWorksQuery.Title!;
                    }

                    if (resultPageSession.Success)
                        _searchWorksQuery.Take = resultPageSession.Value;
                    _useStoredQuery = _searchWorksQuery.IsValid;
                    if (_searchWorksQuery.IsValid)
                        await InitiateSearch();
                }
            }
            catch (Exception ex)
            {
                Console?.LogError($"InitSearchWorksQuery => {ex}");
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_discriminatorSelector.SelectedValues == null || !_discriminatorSelector.SelectedValues.Any())
            {
                _discriminatorSelector.SelectedValues = new List<string>()
                {
                    Discriminator.Series.ToString(),
                    Discriminator.Season.ToString(),
                    Discriminator.Episode.ToString(),
                    Discriminator.StandAlone.ToString()
                };
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (FilterClientID <= 0) return;

            var clients = await RefDataService.SearchClients(FilterClientID, null);
            SelectedClient = clients.FirstOrDefault();
            if (SelectedClient == null) return;

            if (FilterCatalogueID <= 0) return;
            selectedCatalogue = SelectedClient.Catalogues.SingleOrDefault(c => c.Id == FilterCatalogueID);
            searchString = string.Empty;
        }

        private async Task<GridData<WorksDto>> ServerReload(GridState <WorksDto> state)
        {
            if (IsBusy || !_userSearch && (!_useStoredQuery && !_autoCompleteSearch && !_searchWorksQuery.IsValid))
                return new GridData<WorksDto>() { TotalItems = totalItems, Items = RefDataService.Empty<WorksDto>() };

            await SetStatusAsync(true, "Loading");
            try
            {
                if (_worksTable == null)
                {
                    _worksTable = _autoCompleteSearch
                                        ? await GetAutoCompleteSearchResults()
                                        : await GetSearchWorksQueryResults(state);
                }

                if (_worksTable != null)
                {
                    totalItems = _worksTable.TotalRecords;

                    
                    var worksDtos = state.SortDirection == SortDirection.Ascending ?
                                    _worksTable.Records.ToArray().OrderByDynamic(c => $"c.{state.SortLabel}").ToList() :
                                    _worksTable.Records.ToArray().OrderByDescendingDynamic(c => $"c.{state.SortLabel}").ToList();

                    _pagedData = worksDtos.Skip(state.Page * state.PageSize).Take(state.PageSize);
                }

                _worksData = new GridData<WorksDto>() { TotalItems = totalItems, Items = _pagedData };
                return _worksData;
            }
            finally
            {
                _userSearch = false;
                await SetStatusAsync(false, "Loaded");
            }
        }

        private async Task<IEntityTable<WorksDto>> GetSearchWorksQueryResults(TableState state)
        {
            if (_useStoredQuery && _searchWorksQuery != null)
            {
                PopulateSearchPanel(state);
                _useStoredQuery = false;
            }
            else
            {
                SetSearchPerson();
                _searchWorksQuery = CreateSearchWorksQuery(state);
                if (!_searchWorksQuery.IsValid)
                {
                    if (!await IsValidated("Search"))
                    {
                        Console?.Log("Search Criteria", "User canceled search");
                        return EntityTable<WorksDto>.Create(new List<WorksDto> { }).WithTotal(0);
                    }
                }
                try
                {
                    await ProtectedLocalStore.SetAsync("SearchWorksQuery", _searchWorksQuery);
                }
                catch (Exception ex)
                {
                    Console.LogException(ex);
                }
            }

            return (await Mediator.Send(_searchWorksQuery)).Value;
        }

        private void PopulateSearchPanel(TableState state)
        {
            searchString = _searchWorksQuery.Title!;
            state.PageSize = _searchWorksQuery.Take;
            state.Page = _searchWorksQuery.Start / _searchWorksQuery.Take;

            state.SortLabel = _searchWorksQuery.SortColumn ?? string.Empty;
            state.SortDirection = _searchWorksQuery.SortDirection == SearchDirection.Ascending
                ? SortDirection.Ascending
                : SortDirection.Descending;

            RowsPerPageSession = _searchWorksQuery.Take;
            _grid.CurrentPage = _searchWorksQuery.Start / _searchWorksQuery.Take;
            _grid.SortLabel = _searchWorksQuery.SortColumn ?? string.Empty;

            _searchDiscriminators = GetDiscriminators(_searchWorksQuery.Discriminators);
            _discriminatorSelector.SelectedValues = _searchWorksQuery.Discriminators.Select(d => d.ToString());

            if (!string.IsNullOrEmpty(_searchWorksQuery.DirectorFirstName) ||
                !string.IsNullOrEmpty(_searchWorksQuery.DirectorLastName))
            {
                _searchPerson = PersonType.Director;
                searchStringFirstName = searchStringDF = _searchWorksQuery.DirectorFirstName ?? string.Empty;
                searchStringSurname = searchStringDS = _searchWorksQuery.DirectorLastName ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(_searchWorksQuery.ActorFirstName) ||
                !string.IsNullOrEmpty(_searchWorksQuery.ActorLastName))
            {
                _searchPerson = PersonType.Actor;
                searchStringFirstName = searchStringAF = _searchWorksQuery.ActorFirstName ?? string.Empty;
                searchStringSurname = searchStringAS = _searchWorksQuery.ActorLastName ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(_searchWorksQuery.ProducerFirstName) ||
                !string.IsNullOrEmpty(_searchWorksQuery.ProducerLastName))
            {
                _searchPerson = PersonType.Producer;
                searchStringFirstName = searchStringPF = _searchWorksQuery.ProducerFirstName ?? string.Empty;
                searchStringSurname = searchStringPS = _searchWorksQuery.ProducerLastName ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(_searchWorksQuery.ScreenWriterFirstName) ||
                !string.IsNullOrEmpty(_searchWorksQuery.ScreenWriterLastName))
            {
                _searchPerson = PersonType.ScreenWriter;
                searchStringFirstName = searchStringSF = _searchWorksQuery.ScreenWriterFirstName ?? string.Empty;
                searchStringSurname = searchStringSS = _searchWorksQuery.ScreenWriterLastName ?? string.Empty;
            }

            _statusDiscriminator = _searchWorksQuery.StatusDiscriminator;
            productionYear = _searchWorksQuery.ProductionYear;
            _searchTypeDiscriminator = _searchWorksQuery.SearchType;
            _worksTypeId = _searchWorksQuery.WorksTypeId;
            _hasNoRights = _searchWorksQuery.HasNoRights ?? false;
            _createFromDate = _searchWorksQuery.DateCreatedFrom;
            _createToDate = _searchWorksQuery.DateCreatedTo;
            searchStringAgicoaRef = _searchWorksQuery.SearchStringAgicoaRef ?? string.Empty;
            searchStringCompactRef = _searchWorksQuery.SearchStringCompactRef ?? string.Empty;
            searchStringAS400 = _searchWorksQuery.SearchStringAS400 ?? string.Empty;

            if (FilterClientID > 0 && _searchWorksQuery.ClientID.GetValueOrDefault(FilterClientID) != FilterClientID)
            {
                _searchWorksQuery.ClientID = FilterClientID;
            }

            if (_searchWorksQuery.ClientID.HasValue)
            {
                SelectedClient = RefDataService.GetClient(_searchWorksQuery.ClientID.Value);
            }

            if (_searchWorksQuery.CatalogueId.HasValue && SelectedClient != null)
            {
                selectedCatalogue = SelectedClient.Catalogues.FirstOrDefault(c => c.Id == _searchWorksQuery.CatalogueId);
            }

            if (_searchWorksQuery.CountryID.HasValue)
            {
                SelectedCountry = RefDataService.GetCountry(_searchWorksQuery.CountryID.Value);
            }

            if (_searchWorksQuery.RightsCountryID.HasValue)
            {
                SelectedRightsCountry = RefDataService.GetCountry(_searchWorksQuery.RightsCountryID.Value);
            }

            if (_searchWorksQuery.ProductionCompanyID.HasValue)
            {
                _productionCompany = RefDataService.GetCompany(_searchWorksQuery.ProductionCompanyID.Value);
            }
        }

        private SearchWorksQuery CreateSearchWorksQuery(TableState state)
        {
            return new SearchWorksQuery()
            {
                Title = searchString,
                Start = state.Page * state.PageSize,
                Take = state.PageSize,
                SortColumn = !string.IsNullOrWhiteSpace(state.SortLabel) ? state.SortLabel : "DisplayTitle",
                SortDirection = (state.SortDirection == SortDirection.Ascending)
                    ? SearchDirection.Ascending
                    : SearchDirection.Descending,
                Discriminators = GetDiscriminators(),
                ClientID = SelectedClient?.Id,
                CatalogueId = selectedCatalogue?.Id,
                DirectorFirstName = searchStringDF,
                DirectorLastName = searchStringDS,
                ActorFirstName = searchStringAF,
                ActorLastName = searchStringAS,
                ProducerFirstName = searchStringPF,
                ProducerLastName = searchStringPS,
                ScreenWriterFirstName = searchStringSF,
                ScreenWriterLastName = searchStringSS,
                StatusDiscriminator = _statusDiscriminator,
                CountryID = SelectedCountry?.Id,
                ProductionYear = productionYear,
                RightsCountryID = SelectedRightsCountry?.Id,
                SearchType = _searchTypeDiscriminator,
                WorksTypeId = _worksTypeId,
                HasNoRights = _hasNoRights ? _hasNoRights : null,
                DateCreatedFrom = _createFromDate,
                DateCreatedTo = _createToDate,
                SearchStringAgicoaRef = searchStringAgicoaRef,
                SearchStringCompactRef = searchStringCompactRef,
                SearchStringAS400 = searchStringAS400,
                ProductionCompanyID = _productionCompany?.Id
            };
        }

        private List<Discriminator> GetDiscriminators()
        {
            List<Discriminator> discriminators = new List<Discriminator>();
            var searchDiscriminatorStringList = _searchDiscriminators.Split(',').Where(x => x != string.Empty).ToList();

            foreach (var discriminator in searchDiscriminatorStringList)
            {
                discriminators.Add((Discriminator)Enum.Parse(typeof(Discriminator), discriminator, true));
            }

            return discriminators;
        }

        private string GetDiscriminators(List<Discriminator> discriminators)
        {
            var discriminatorsString = discriminators.Select(d => d.ToString());
            return string.Join(",", discriminatorsString);
        }

        private void SetSearchPerson()
        {
            ResetSearchNameStrings();
            switch (_searchPerson)
            {
                case PersonType.Actor:
                    searchStringAF = searchStringFirstName;
                    searchStringAS = searchStringSurname;
                    break;
                case PersonType.Director:
                    searchStringDF = searchStringFirstName;
                    searchStringDS = searchStringSurname;
                    break;
                case PersonType.Producer:
                    searchStringPF = searchStringFirstName;
                    searchStringPS = searchStringSurname;
                    break;
                case PersonType.ScreenWriter:
                    searchStringSF = searchStringFirstName;
                    searchStringSS = searchStringSurname;
                    break;
            }
        }

        private void ResetSearchNameStrings()
        {
            searchStringDF = "";
            searchStringDS = "";
            searchStringAF = "";
            searchStringAS = "";
            searchStringPF = "";
            searchStringPS = "";
            searchStringSF = "";
            searchStringSS = "";
        }

        protected override async Task LoadStateAsync()
        {
            await base.LoadStateAsync();
            
            var resultColumnsSession = await ProtectedLocalStore.GetAsync<HashSet<WorksListColumn?>>("WorksListColumn");
            if (resultColumnsSession.Success && resultColumnsSession.Value != null)
            {
                columnsVisible = resultColumnsSession.Value;
            }

            var fixedHeader = await ProtectedLocalStore.GetAsync<bool>("WorksFixedHeader");
            if (fixedHeader.Success) _fixedHeader = fixedHeader.Value;
            var fixedFooter = await ProtectedLocalStore.GetAsync<bool>("WorksFixedFooter");
            if (fixedFooter.Success) _fixedFooter = fixedFooter.Value;
            SetPageSizeFromSessionState();
            SetAdditionalColumnsFromSessionState();
        }

        private async void UpdateWorksColumnsInSession(IEnumerable<WorksListColumn?> newWorksListColumn)
        {
            columnsVisible = newWorksListColumn;

            try
            {
                await ProtectedLocalStore.SetAsync("WorksListColumn", newWorksListColumn);
            }
            catch (Exception ex)
            {
                Console.LogException(ex);
            }
        }

        protected override void SetPageSizeFromSessionState()
        {
            if (_grid != null && _grid.RowsPerPage != RowsPerPageSession)
            {
                _grid.SetRowsPerPage(RowsPerPageSession);
            }
        }

        private void SetAdditionalColumnsFromSessionState()
        {
            if (_worksListColumnSelect != null && columnsVisible != null &&
                _worksListColumnSelect.SelectedValues != columnsVisible)
            {
                _worksListColumnSelect.SelectedValues = columnsVisible;
            }
        }

        protected override async Task SaveStateAsync()
        {
            try
            {
                RowsPerPageSession = _grid?.RowsPerPage ?? 25;
                await ProtectedLocalStore.SetAsync("WorksFixedHeader", _fixedHeader);
                await ProtectedLocalStore.SetAsync("WorksFixedFooter", _fixedFooter);
                await base.SaveStateAsync();
            }
            catch (Exception ex)
            {
                Console.LogException(ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadStateAsync();
                _advancedSearchPanel?.Expand();
            }
        }

        private void CopyFormComplete(CopyFormResponse response)
        {
            openCopyDrawer = false;

            if (response.Response)
            {
                var actionString = "Copied";

                if (response.Relinquish)
                    actionString = "Moved";

                actionText = selectedItems.Count() + " Works " + actionString + " to " + response.Client.ClientName;

                if (response.Catalogue != null)
                    actionText += "/" + response.Catalogue.Name;

                if (response.Response)
                    Snackbar.Add(actionText);

                selectedItems = new HashSet<WorksDto>();
                StateHasChanged();
            }
        }

        public async Task onFilterClear(EventArgs args)
        {
            FilterClientID = 0;
            FilterCountryID = 0;
            SelectedCountry = null;
            SelectedRightsCountry = null;
            _rightsCountryAutoComplete?.Clear();
            _countryAutoComplete?.Clear();
            _productionCompanyAutoComplete?.Clear();

            searchString = "";
            searchStringDF = "";
            searchStringDS = "";
            searchStringAF = "";
            searchStringAS = "";
            searchStringPF = "";
            searchStringPS = "";
            searchStringSF = "";
            searchStringSS = "";
            searchStringFirstName = "";
            searchStringSurname = "";
            searchStringAgicoaRef = "";
            searchStringCompactRef = "";
            searchStringAS400 = "";
            SelectedClient = null;
            _discriminator = Discriminator.All;
            _discriminatorSelector.SelectedValues = new List<string>()
            {
                Discriminator.Series.ToString(),
                Discriminator.Season.ToString(),
                Discriminator.Episode.ToString(),
                Discriminator.StandAlone.ToString()
            };
            _statusDiscriminator = WorksStatus.Any;
            productionYear = null;
            _productionCompany = null;

            selectedCatalogue = null;

            _hasNoRights = false;
            _noRightsSwitch.Checked = false;
            _worksTypeId = null;
            await _worksTypePicker.Clear();

            if (clientSelect != null)
                await clientSelect.Clear();
            _searchTypeDiscriminator = SearchType.Contains;
            await _autoComplete.Clear();

            _createFromDate = null;
            _createToDate = null;
            _createFromPicker.Clear();
            _createToPicker.Clear();

            _useStoredQuery = false;
            await ProtectedLocalStore.DeleteAsync("SearchWorksQuery");

            StateHasChanged();

            _worksData = new TableData<WorksDto> { TotalItems = 0, Items = new List<WorksDto>() };
        }

        public async Task onExportResults(EventArgs args)
        {
            await Task.Delay(1);

            await SetStatusAsync(true, "Fetching Data to Export");

            switch (_searchPerson)
            {
                case PersonType.Actor:
                    searchStringAF = searchStringFirstName;
                    searchStringAS = searchStringSurname;
                    break;
                case PersonType.Director:
                    searchStringDF = searchStringFirstName;
                    searchStringDS = searchStringSurname;
                    break;
                case PersonType.Producer:
                    searchStringPF = searchStringFirstName;
                    searchStringPS = searchStringSurname;
                    break;
                case PersonType.ScreenWriter:
                    searchStringSF = searchStringFirstName;
                    searchStringSS = searchStringSurname;
                    break;
            }

            var searchQuery = new SearchWorksQuery()
            {
                Title = searchString,
                Start = 0,
                Take = 0,
                SortColumn = !string.IsNullOrWhiteSpace(_sortColumn) ? _sortColumn : "DisplayTitle",
                Discriminators = GetDiscriminators(),
                ClientID = SelectedClient?.Id,
                CatalogueId = selectedCatalogue?.Id,
                DirectorFirstName = searchStringDF,
                DirectorLastName = searchStringDS,
                ActorFirstName = searchStringAF,
                ActorLastName = searchStringAS,
                ProducerFirstName = searchStringPF,
                ProducerLastName = searchStringPS,
                ScreenWriterFirstName = searchStringSF,
                ScreenWriterLastName = searchStringSS,
                StatusDiscriminator = _statusDiscriminator,
                CountryID = SelectedCountry?.Id,
                ProductionYear = productionYear,
                RightsCountryID = SelectedRightsCountry?.Id,
                SearchType = _searchTypeDiscriminator,
                WorksTypeId = _worksTypeId,
                HasNoRights = _hasNoRights ? _hasNoRights : null,
                DateCreatedFrom = _createFromDate,
                DateCreatedTo = _createToDate,
                SearchStringAgicoaRef = searchStringAgicoaRef,
                SearchStringCompactRef = searchStringCompactRef,
                SearchStringAS400 = searchStringAS400,
                ProductionCompanyID = _productionCompany?.Id
            };
            
            if (!searchQuery.IsValid)
            {
                if (!await IsValidated("Export"))
                {
                    Console?.Log("Search Criteria", "User canceled export");
                    await SetStatusAsync(false, "User canceled export");
                    return;
                }
            }
            var worksToExport = (await Mediator.Send(searchQuery)).Value;
            Snackbar.Add($"{worksToExport.TotalRecords} records retrieved. Please DO NOT navigate away from this page",
                (worksToExport.TotalRecords > maxNumberOfRecordsForCsvExport) ? Severity.Warning : Severity.Info);
            await Task.Delay(1);

            await SetStatusAsync(true, "Preparing to Export");
            await BrowserDownload.ExportWorksAsCsv(worksToExport.Records, "WorksResultsExport.csv");

            await SetStatusAsync(false, $"Exported {worksToExport.TotalRecords} records");
        }

        private async Task InitiateSearch()
        {
            _autoCompleteSearch = false;
            _userSearch = true;
            _worksTable = null;
            if (!string.IsNullOrWhiteSpace(searchString))
                searchString = searchString.Replace("\"", "");
            await _grid.ReloadServerData();
            await SaveStateAsync();
        }

        protected void OpenWorksDrawer(int id, Discriminator discriminator, string title)
        {
            NavigationManager.NavigateTo($"WorksDetail/{discriminator}");
            //worksId = id;
            //_discriminator = discriminator;
            //openWorksDrawer = true;
            //StateHasChanged();
        }

        private void OpenCopyDrawer()
        {
            openCopyDrawer = true;
        }

        private void OpenRightsForm(int id)
        {
            openRightsDrawer = true;
            worksId = id;
        }

        protected async Task SuccessRightsCallback()
        {
            openRightsDrawer = false;
            await Task.CompletedTask;
        }

        private string GetPersonName(List<PersonDto> persons)
        {
            var person = persons?.FirstOrDefault();

            if (persons != null && persons.Any())
            {
                if (!string.IsNullOrWhiteSpace(searchStringDF) && !string.IsNullOrWhiteSpace(searchStringDS))
                {
                    person = persons.FirstOrDefault(p =>
                        p.FirstName.Contains(searchStringFirstName, StringComparison.InvariantCultureIgnoreCase)
                        || p.LastName.Contains(searchStringSurname, StringComparison.InvariantCultureIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(searchStringFirstName) && string.IsNullOrWhiteSpace(searchStringSurname))
                {
                    person = persons.FirstOrDefault(p =>
                        p.FirstName.Contains(searchStringFirstName, StringComparison.InvariantCultureIgnoreCase));
                }

                if (string.IsNullOrWhiteSpace(searchStringFirstName) && !string.IsNullOrWhiteSpace(searchStringSurname))
                {
                    person = persons.FirstOrDefault(p =>
                        p.LastName.Contains(searchStringSurname, StringComparison.InvariantCultureIgnoreCase));
                }

                if (person == null)
                {
                    person = persons.FirstOrDefault();
                }
            }

            return person == null ? "" : person.FirstName + " " + person.LastName;
        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return string.Join(",", selectedValues);
        }

        protected async Task ToggleWorksDrawer(bool open)
        {
            openWorksDrawer = open;
            await Task.CompletedTask;
        }

        public void RowDoubleClicked(TableRowClickEventArgs<WorksDto> eventArgs)
        {
            if (eventArgs.MouseEventArgs.Detail <= 1) return;
            NavigationManager.NavigateTo($"WorksDetail/{eventArgs.Item.Discriminator}/{eventArgs.Item.Id}");
        }

        private void CloseInfoBanner(bool value)
        {
            _showInfoBanner = !value;
            StateHasChanged();
        }

        private static object SortPersonsByName(ICollection<PersonDto> items)
        {
            if (items == null) return string.Empty;
            return items.Select(x => $"{x.FirstName} {x.LastName}");
        }

        private Func<WorksDto, object> SortClientsByName = item =>
        {
            if (item == null || item.Clients == null) return string.Empty;
            return item.Clients.OrderBy(x=> x.ClientName).Select(x => x.ClientName);
        };

        private Func<WorksDto, object> SortCataloguesByName = item =>
        {
            if (item == null || item.Catalogues == null) return string.Empty;
            return item.Catalogues.Select(x => x.Name);
        };

        private Func<WorksDto, object> SortTitlesByName = item =>
        {
            if (item == null || item.Titles == null) return string.Empty;
            return item.Titles.Where(_mainTitlesPredicate).OrderBy(x=> x.Title).Select(x => x.Title);
        };

        private Func<WorksDto, object> SortAltTitlesByName = item =>
        {
            if (item == null || item.Titles == null) return string.Empty;
            return item.Titles.Where(_alternateTitlesPredicate).OrderBy(x => x.Title).Select(x => x.Title);
        };

        private Func<WorksDto, object> SortActorsByName = item =>
        {
            if (item == null || item.Actors == null) return string.Empty;
            return SortPersonsByName(item.Actors);
        };

        private Func<WorksDto, object> SortDirectorsByName = item =>
        {
            if (item == null || item.Directors == null) return string.Empty;
            return SortPersonsByName(item.Directors);
        };

        private Func<WorksDto, object> SortProducersByName = item =>
        {
            if (item == null || item.Producers == null) return string.Empty;
            return SortPersonsByName(item.Producers);
        };

        private Func<WorksDto, object> SortScreenWritersByName = item =>
        {
            if (item == null || item.ScreenWriters == null) return string.Empty;
            return SortPersonsByName(item.ScreenWriters);
        };

        public async void Enter(KeyboardEventArgs e)
        {
            if (e.Code == C_Enter_Key || e.Code == C_NumpadEnter_Key)
            {
                await InitiateSearch();
            }
        }

        protected async Task<bool> GetUserConfirmation(string message, string title = "Validation", string yesText = "Continue", string cancelText = "Cancel")
        {
            if (string.IsNullOrWhiteSpace(message)) return true;
            var htmlMessage = (MarkupString)$"{message}<br /><b>Please provide search criteria and try again.</b>";
            MessageBoxOptions options = new MessageBoxOptions() { MarkupMessage = htmlMessage, Title = title };
            await DialogService.ShowMessageBox(options);
            Console?.Log(title, $"Message: {message}");
            return false;
        }

        protected async Task<bool> IsValidated(string operation="Search")
        {
            var message = $"<b>Warning</b>: {operation} without Search criteria is not supported.";
            return await GetUserConfirmation(message);
        }

        private bool IsTitlesMatched(ICollection<WorksTitleDto> titles, bool alternate= false)
        {
            if (titles == null) return false;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                return (titles.Any(t => alternate ? _alternateTitlesPredicate(t) : _mainTitlesPredicate(t) && t.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }

            return false;
        }
        private List<string> GetMatchedTitles(ICollection<WorksTitleDto> titles, bool alternate = false)
        {
            var result = new List<string>();
            if (titles == null) return result;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var list = titles
                    .Where(t => (alternate
                        ? _alternateTitlesPredicate(t)
                        : _mainTitlesPredicate(t)) && 
                                t.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .Select(t => t.Title).ToList();
                result.AddRange(list);
            }
            //else
            //{
            //    result.AddRange(titles.Where(alternate ? _alternateTitlesPredicate : _mainTitlesPredicate).Select(t => t.Title)!);
            //}
            return result;
        }
        private string GetAllTitles(ICollection<WorksTitleDto> titles)
        {
            if (titles == null) return string.Empty;

            if (titles.Any())
            {
                return string.Join(", ", titles.Select(t => t.Title));
            }
            return string.Empty;
        }

        private string GetTitles(ICollection<WorksTitleDto> titles, bool alternate = false)
        {
            if (titles == null) return string.Empty;

            if (titles.Any(alternate ? _alternateTitlesPredicate : _mainTitlesPredicate))
            {
                return string.Join(", ", titles.Where(alternate ? _alternateTitlesPredicate : _mainTitlesPredicate).Select(t => t.Title));
            }
            return string.Empty;
        }

        private string GetMainTitles(ICollection<WorksTitleDto> titles) => GetTitles(titles, false);
        private string GetAlternateTitles(ICollection<WorksTitleDto> titles) => GetTitles(titles, true);
    }
}
