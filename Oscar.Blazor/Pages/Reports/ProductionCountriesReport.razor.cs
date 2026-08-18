using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Oscar.Blazor.Pages.Reports
{
    public partial class ProductionCountriesReport : OscarReportComponent
    {
        #region - Private Declarations -
        private ClientDto? _selectedClient;
        private CatalogueDto? _selectedCatalogue;
        //private CountryDto? _selectedCountry;
        private MudAutocomplete<ClientDto>? _clientSelect;
        private MudAutocomplete<CountryDto>? _countrySelect;
        private MudTable<ProductionCountryItem>? _dataGrid;
        private List<ProductionCountryItem>? _gridItems = new();
        private ProductionCountryItem? _selectedItem;
        //private readonly HashSet<CountryDto?> _selectedCountries = new HashSet<CountryDto?>();
        private IReadOnlyCollection<string> _selectedWorksStatuses = new HashSet<string> { "ACTIVE" };
        private IReadOnlyCollection<string> _selectedDiscriminators = new HashSet<string> { "Episode", "Season", "Series", "StandAlone" };
        private List<string> _countries = new List<string>();
        private readonly Dictionary<string, int> _workStatusLookup = new()
        {
            { "ACTIVE", 1 },
            { "UNCONTROLLED", 2 },
            { "INCOMPLETE", 3 },
            { "RELINQUISHED", 4 }
        };
        private readonly string[] _workStatuses = { "ACTIVE", "UNCONTROLLED", "INCOMPLETE", "RELINQUISHED" };
        private readonly string[] _discriminators = { "Episode", "Season", "Series", "StandAlone" };

        private string _selectedCountryName = string.Empty;
        private IReadOnlyCollection<string> _selectedCountryNames = new HashSet<string>();
        #endregion

        #region - Clients -
        protected ClientDto? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                PopulateClientDetails();
            }
        }
        protected async Task<IEnumerable<ClientDto>> ClientSearch(string searchTerm, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SelectedClient = null;
                return RefDataService.Empty<ClientDto>();
            }
            return await RefDataService.SearchClients(null, searchTerm);
        }

        private string ClientDisplayFormat(ClientDto? client)
        {
            return client != null ? $"{client.ClientName} ({client.Status})" : "";
        }

        private void PopulateClientDetails()
        {
            if (_selectedClient != null)
            {
                RefDataService.GetClientCatalogs(_selectedClient.Id, results => { _selectedClient.Catalogues = results.ToCollection(); });
            }
        }
        #endregion

        #region - Countries -
        /*
        protected CountryDto? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                AddSelectedCountry(value);
            }
        }

        private async Task<IEnumerable<CountryDto>> CountrySearch(string searchTerm, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RefDataService.Empty<CountryDto>();
            }
            return await RefDataService.SearchCountries(searchTerm);
        }

        private string CountryDisplayFormat(CountryDto? item)
        {
            return item != null ? $"{item.Name} ({item.Code})" : "";
        }

        private void RemoveSelectedCountry(MudChip<string> chip)
        {
            _selectedCountries.RemoveWhere(x => GetCountryName(x) == chip.Text);
        }

        private void AddSelectedCountry(CountryDto? newValue)
        {
            if (newValue != null)
            {
                _selectedCountries.Add(newValue);
                _selectedCountry = null;
            }
        }

        private string GetCountryName(CountryDto? item) => item != null ? item.Name : string.Empty;
        private int GetCountryId(CountryDto? item) => item?.Id ?? -1;
        */
        #endregion

        #region - Grid Filters -
        private bool GridFilterFunc(ProductionCountryItem? item)
        {
            if (item == null) return true;
            return item.FilterBy(SearchText);
        }
        #endregion

        #region - Method Overrides -

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _countries.Clear();
            var countries = await RefDataService.GetCountries();
            _countries.AddRange(countries.Select(x => x.Name));
        }

        public override void InitReportDefaults()
        {
            ReportName = nameof(ProductionCountriesReport);
            ReportTitle = "Production Countries Report";
        }

        public override async Task ResetSearchCriteria()
        {
            SelectedClient = null;
            //SelectedCountry = null;

            _selectedCatalogue = null;
            _selectedWorksStatuses = new HashSet<string> { "ACTIVE" };
            _selectedDiscriminators = new HashSet<string> { "Episode", "Season", "Series", "StandAlone" };
            if (_clientSelect != null) { await _clientSelect.ClearAsync(); }
            if (_countrySelect != null) { await _countrySelect.ClearAsync(); }

            _selectedCountryNames = new HashSet<string>();
            _gridItems?.Clear();
            StateHasChanged();
        }

        protected override string Validate()
        {
            if (SelectedClient == null) return "You must select Client to proceed.";
            return string.Empty;
        }

        protected override async Task<bool> IsUserConfirmed(string operation)
        {
            if (SelectedClient != null) return true;
            var message = $"<b>Warning</b>: {operation} without a Client selected may take more time. Do you wish to continue?";
            return await GetUserConfirmation(message, "Confirm");
        }

        public override async Task ExecuteReportSearch()
        {
            var items = await ReportService.GetProductionCountriesItems(GetUserQuery());
            if (items is { } data)
            {
                _gridItems = data.ToList();
                Log("RunReport", $"Retrieved {_gridItems.Count} records");
            }
        }

        public override async Task<FileStreamResult> ExportReportToExcel()
        {
            return await ReportService.ExportProductionCountryItems(ExportType.Excel, GetUserQuery(), ReportName);
        }

        public override async Task<FileStreamResult> ExportReportToCsv()
        {
            return await ReportService.ExportProductionCountryItems(ExportType.Csv, GetUserQuery(), ReportName);
        }
        #endregion

        #region - Private Methods -
        private Query GetUserQuery()
        {
            var query = new Query { OrderBy = "ClientName asc" };
            int paramIndex = 0;
            if (SelectedClient != null)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"i => i.ClientId = @{paramIndex++}", SelectedClient.Id);

                if (_selectedCatalogue != null)
                {
                    query.AddOrAppendFilter(LogicalFilterOperator.And, $"i.CatalogueId = @{paramIndex++}", _selectedCatalogue.Id);
                }
            }

            if (_selectedWorksStatuses.Any() && _selectedWorksStatuses.Count() != _workStatuses.Length)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"WorksStatusId IN ({string.Join(",", GetSelectedWorkStatusIds())})");
            }
            if (_selectedDiscriminators.Any() && _selectedDiscriminators.Count() != _discriminators.Length)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"Discriminator IN (\"{string.Join("\", \"", _selectedDiscriminators)}\")");
            }
            if (_selectedCountryNames.Any() && _selectedCountryNames.Count() != _countries.Count)
            {
                foreach (var countryName in _selectedCountryNames)
                {
                    query.AddOrAppendFilter(LogicalFilterOperator.And, $"i.CountriesOfProduction.Contains(@{paramIndex++})", countryName);
                }
            }
            Log("Search Criteria", query.Filter);
            return query;
        }

        private List<int> GetSelectedWorkStatusIds()
        {
            var result = new List<int> { -1 }; // Set Default Any
            foreach (var workStatus in _selectedWorksStatuses)
            {
                result.Add(_workStatusLookup[workStatus]);
            }

            return result;
        }
        #endregion
    }
}
