using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Blazor.Pages.Reports
{
    public partial class WorksListReport : OscarReportComponent
    {
        #region - Private Declarations -
        private ClientDto? _selectedClient;
        private CatalogueDto? _selectedCatalogue;
        private MudAutocomplete<ClientDto>? _clientSelect;
        private MudDataGrid<ClientWorkItem>? _dataGrid;
        private List<ClientWorkItem>? _gridItems = new List<ClientWorkItem>();
        private ClientWorkItem? selectedItem = null;
        private string _selectedWorkStatus;
        private string _selectedDiscriminator;
        private IReadOnlyCollection<string> _selectedWorksStatuses = new HashSet<string> { "ACTIVE" };
        private IReadOnlyCollection<string> _selectedDiscriminators = new HashSet<string>();

        private readonly string[] _workStatuses = { "ACTIVE", "UNCONTROLLED", "INCOMPLETE", "RELINQUISHED" };
        private readonly string[] _discriminators = { "Episode", "Season", "Series", "StandAlone" };
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

        #region - Grid Filters -
        private bool FilterFunc(ClientWorkItem? item) => FilterFunc(item, SearchText);

        private bool FilterFunc(ClientWorkItem? item, string searchString)
        {
            if (item == null) return true;
            return item.FilterBy(searchString);
        }
        #endregion

        #region - Method Overrides -
        public override void InitReportDefaults()
        {
            ReportName = nameof(WorksListReport);
            ReportTitle = "Works List Report";
        }

        public override async Task ResetSearchCriteria()
        {
            SelectedClient = null;
            _selectedCatalogue = null;
            _selectedDiscriminator = string.Empty;
            _selectedWorkStatus = string.Empty;
            _selectedWorksStatuses = new HashSet<string> { "ACTIVE" };
            _selectedDiscriminators = new HashSet<string>();
            if (_clientSelect != null) await _clientSelect.ClearAsync();
        }

        protected override async Task<bool> IsUserConfirmed(string operation)
        {
            if (SelectedClient != null) return true;
            var message = $"<b>Warning</b>: {operation} without a Client selected may take more time.<br /><b>Do you wish to continue?</b>";
            return await GetUserConfirmation(message, "Confirm");
        }

        public override async Task ExecuteReportSearch()
        {
            var items = await ReportService.GetClientWorksList(GetUserQuery());
            if (items is { } data)
            {
                _gridItems = data.ToList();
                Log("RunReport", $"Retrieved {_gridItems.Count} records");
            }
        }

        public override async Task<FileStreamResult> ExportReportToExcel()
        {
            return await ReportService.ExportClientWorksList(ExportType.Excel, GetUserQuery(), ReportName);
        }

        public override async Task<FileStreamResult> ExportReportToCsv()
        {
            return await ReportService.ExportClientWorksList(ExportType.Csv, GetUserQuery(), ReportName);
        }
        #endregion

        #region - Private Methods -
        private Query GetUserQuery()
        {
            int paramIndex = 0;
            var query = new Query { OrderBy = "ClientName asc" };
            if (SelectedClient != null)
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"i => i.ClientId = @{paramIndex++}", SelectedClient.Id);
                if (_selectedCatalogue != null)
                {
                    query.AddOrAppendFilter(LogicalFilterOperator.And, $"i.CatalogueId = @{paramIndex++}", _selectedCatalogue.Id);
                }
            }

            if (_selectedWorksStatuses.Any())
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"WorksStatus IN (\"{string.Join("\", \"", _selectedWorksStatuses)}\")");
            }
            if (_selectedDiscriminators.Any())
            {
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"Discriminator IN (\"{string.Join("\", \"", _selectedDiscriminators)}\")");
            }
            Log("Search Criteria", query.Filter);
            return query;
        }


        #endregion
    }
}
