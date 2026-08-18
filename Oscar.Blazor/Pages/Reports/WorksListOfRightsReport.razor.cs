using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Blazor.Pages.Reports
{
    public partial class WorksListOfRightsReport: OscarReportComponent
    {

        #region - Private Declarations -
        private ClientDto? _selectedClient;
        private CatalogueDto? _selectedCatalogue;
        private MudAutocomplete<ClientDto>? _clientSelect;
        //private MudTable<ClientWorkRightItem>? _dataGrid;
        private MudDataGrid<ClientWorkRightItem>? _dataGrid;
        private List<ClientWorkRightItem>? _gridItems = new List<ClientWorkRightItem>();
        private ClientWorkRightItem? _selectedItem = null;
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
        private bool FilterFunc(ClientWorkRightItem? item) => FilterFunc(item, SearchText);

        private bool FilterFunc(ClientWorkRightItem? item, string searchString)
        {
            if (item == null) return true;
            return item.FilterBy(searchString);
        }
        #endregion

        #region - Method Overrides -
        public override void InitReportDefaults()
        {
            ReportName = nameof(WorksListOfRightsReport);
            ReportTitle = "Works List Of Rights Report";
        }

        public override async Task ResetSearchCriteria()
        {
            SelectedClient = null;
            _selectedCatalogue = null;
            if (_clientSelect != null) await _clientSelect.ClearAsync();
            _gridItems?.Clear();
            _dataGrid?.ClearFiltersAsync();
        }

        protected override async Task<bool> IsUserConfirmed(string operation)
        {
            if (SelectedClient != null) return true;
            var message = $"<b>Warning</b>: {operation} without a Client selected may take more time.<br /><b>Do you wish to continue?</b>";
            return await GetUserConfirmation(message, "Confirm");
        }

        public override async Task ExecuteReportSearch()
        {
            var items = await ReportService.GetClientWorkRightItems(GetUserQuery());
            if (items is { } data)
            {
                _gridItems = data.ToList();
                Log("RunReport", $"Retrieved {_gridItems.Count} records");
            }
        }

        public override async Task<FileStreamResult> ExportReportToExcel()
        {
            return await ReportService.ExportClientWorkRightItems(ExportType.Excel, GetUserQuery(), ReportName);
        }

        public override async Task<FileStreamResult> ExportReportToCsv()
        {
            return await ReportService.ExportClientWorkRightItems(ExportType.Csv, GetUserQuery(), ReportName);
        }

        protected override string Validate()
        {
            if (SelectedClient == null) return "You must select Client to proceed.";

            return string.Empty;
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
                    query.AddOrAppendFilter(LogicalFilterOperator.And, $"i.CatalogueId = @{paramIndex}", _selectedCatalogue.Id);
                }
            }

            Log("Search Criteria", query.Filter);
            return query;
        }


        #endregion


    }
}
