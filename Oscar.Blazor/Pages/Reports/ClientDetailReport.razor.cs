
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Blazor.Pages.Reports
{
    public partial class ClientDetailReport : OscarReportComponent
    {
        #region - Private Declarations -
        private ClientDto? _selectedClient;
        private OperatorDto? _selectedOperator;
        private CatalogueDto? _selectedCatalogue;
        private MudAutocomplete<ClientDto>? _clientSelect;
        private MudAutocomplete<OperatorDto>? _operatorSelect;
        private bool _includeCatalogues;
        private MudDataGrid<ClientCataloguesDetail>? _clientCataloguesGrid;
        private MudDataGrid<ClientDetail>? _clientsDataGrid;
        private List<ClientCataloguesDetail>? _clientCataloguesItems = new List<ClientCataloguesDetail>();
        private List<ClientDetail>? _clientDetailsItems = new List<ClientDetail>();
        private ClientCataloguesDetail? _selectedClientCatalogueItem = null;
        private ClientDetail? _selectedClientDetailItem = null;
        private IEnumerable<OperatorDto> _operators = new List<OperatorDto>();
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

        public bool IncludeCatalogues
        {
            get => _includeCatalogues;
            set => SetProperty(ref _includeCatalogues, value);
        }
        #endregion

        #region - Operators -
        protected OperatorDto? SelectedOperator
        {
            get => _selectedOperator;
            set => SetProperty(ref _selectedOperator, value);
        }

        protected async Task<IEnumerable<OperatorDto>> OperatorSearch(string searchTerm, CancellationToken token)
        {
            if (_operators == null || !_operators.Any()) { _operators = await RefDataService.GetOperators(); }
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SelectedOperator = null;
                return RefDataService.Empty<OperatorDto>();
            }

            return _operators.Where(x => x.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private string OperatorDisplayFormat(OperatorDto? item)
        {
            return item != null ? item.FullName : "";
        }

        #endregion

        #region - Grid Filters -
        private bool ClientFilterFunc(ClientDetail item) => ClientFilterFunc(item, SearchText);

        private bool ClientFilterFunc(ClientDetail item, string searchString)
        {
            if (item == null) return true;
            return item.FilterBy(searchString);
        }
        private bool ClientCatalogueFilterFunc(ClientCataloguesDetail item) => ClientCatalogueFilterFunc(item, SearchText);

        private bool ClientCatalogueFilterFunc(ClientCataloguesDetail item, string searchString)
        {
            if (item == null) return true;
            return item.FilterBy(searchString);
        }
        #endregion

        #region - Method Overrides -
        public override void InitReportDefaults()
        {
            ReportName = IncludeCatalogues ? nameof(ClientCataloguesDetail) : nameof(ClientDetail);
            ReportTitle = $"Client {(IncludeCatalogues ? "Catalogues " : "")}Detail Report";
        }

        public override async Task ResetSearchCriteria()
        {
            SelectedClient = null;
            SelectedOperator = null;
            _selectedCatalogue = null;
            _includeCatalogues = false;
            if (_clientSelect != null) await _clientSelect.ClearAsync();
            if (_operatorSelect != null) await _operatorSelect.ClearAsync();
            _clientDetailsItems?.Clear();
            _clientCataloguesItems?.Clear();
        }

        public override async Task ExecuteReportSearch()
        {
            if (IncludeCatalogues)
            {
                var items = await ReportService.GetClientCataloguesDetails(GetUserQuery());
                if (items is { } data)
                {
                    _clientCataloguesItems = data.ToList();
                    Log("RunReport", $"Retrieved {_clientCataloguesItems.Count} records");
                }
            }
            else
            {
                var items = await ReportService.GetClientDetails(GetUserQuery());
                if (items is { } data)
                {
                    _clientDetailsItems = data.ToList();
                    Log("RunReport", $"Retrieved {_clientDetailsItems.Count} records");
                }
            }
        }

        public override async Task<FileStreamResult> ExportReportToExcel()
        {
            if (IncludeCatalogues)
            {
                return await ReportService.ExportClientCataloguesDetails(ExportType.Excel, GetUserQuery(), ReportName);
            }
            return await ReportService.ExportClientDetails(ExportType.Excel, GetUserQuery(), ReportName);
        }

        public override async Task<FileStreamResult> ExportReportToCsv()
        {
            if (IncludeCatalogues)
            {
                return await ReportService.ExportClientCataloguesDetails(ExportType.Csv, GetUserQuery(), ReportName);
            }
            return await ReportService.ExportClientDetails(ExportType.Csv, GetUserQuery(), ReportName);
        }
        #endregion

        #region - Private Methods -
        private Query GetUserQuery()
        {
            var query = new Query { OrderBy = "ClientName asc" };
            int paramIndex = 0;
            if (SelectedClient != null)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"ClientId = @{paramIndex++}", SelectedClient.Id);

                if (_selectedCatalogue != null && IncludeCatalogues)
                {
                    query.AddOrAppendFilter(LogicalFilterOperator.And, $"CatalogueId = @{paramIndex++}", _selectedCatalogue.Id);
                }
            }

            if (SelectedOperator != null)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"AccountManager.Equals(@{paramIndex})", SelectedOperator.FullName);
            }

            Log("Search Criteria", query.Filter);
            return query;
        }

        private void OnIncludeCatalogues()
        {
            IncludeCatalogues = !IncludeCatalogues;
            InitReportDefaults();
            StateHasChanged();
        }
        #endregion
    }
}
