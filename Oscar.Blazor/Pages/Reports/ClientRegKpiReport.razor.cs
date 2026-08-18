
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Blazor.Pages.Reports
{
    public partial class ClientRegKpiReport : OscarReportComponent
    {
        #region - Private Declarations -
        private string _selectedYear;
        private ClientDto? _selectedClient;
        private MudAutocomplete<ClientDto>? _clientSelect;
        private MudDataGrid<ClientWorkStatItem>? _dataGrid;
        private List<ClientWorkStatItem>? _gridItems = new List<ClientWorkStatItem>();
        private ClientWorkStatItem? _selectedItem = null;
        #endregion

        #region - Properties -
        protected string SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        protected ClientDto? SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value);
        }

        #endregion

        #region - Clients -
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

        #endregion

        #region - Grid Filters -
        private bool GridFilterFunc(ClientWorkStatItem item) 
        {
            if (item == null) return true;
            return item.FilterBy(SearchText);
        }
        #endregion

        #region - Method Overrides -
        public override void InitReportDefaults()
        {
            ReportName = nameof(ClientRegKpiReport);
            ReportTitle = "Clients Registrations KPI Report by Created Year";
            SelectedYear = $"{DateTime.Now.Year}";
        }

        public override async Task ResetSearchCriteria()
        {
            SelectedClient = null;
            SelectedYear = $"{DateTime.Now.Year}";
            if (_clientSelect != null) await _clientSelect.ClearAsync();
            _gridItems?.Clear();
            await _dataGrid?.ClearFiltersAsync()!;
        }

        public override async Task ExecuteReportSearch()
        {
            var items = await ReportService.GetClientYearlyStats(GetUserQuery());
            if (items is { } data)
            {
                _gridItems = data.ToList();
                Log("RunReport", $"Retrieved {_gridItems.Count} records");
            }
        }

        public override async Task<FileStreamResult> ExportReportToExcel()
        {
            return await ReportService.ExportClientWorkYearlyStats(ExportType.Excel, false, int.Parse(SelectedYear), SelectedClient?.Id, ReportName);
        }

        public override async Task<FileStreamResult> ExportReportToCsv()
        {
            return await ReportService.ExportClientWorkYearlyStats(ExportType.Csv, false, int.Parse(SelectedYear), SelectedClient?.Id, ReportName);
        }

        protected override async Task<bool> IsUserConfirmed(string operation)
        {
            if (SelectedClient != null) return true;
            var message = $"<b>Warning</b>: {operation} without a Client selected may take more time.<br /><b>Do you wish to continue?</b>";
            return await GetUserConfirmation(message, "Confirm");
        }

        protected override string Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedYear)) return $"You must select Created Year to proceed.";

            return string.Empty;
        }
        #endregion

        #region - Private Methods -
        private Query GetUserQuery()
        {
            var query = new Query { OrderBy = "ClientName asc" };
            if (SelectedClient != null)
            {
                query.Filter = $"ClientId = {SelectedClient.Id}";
            }

            if (!string.IsNullOrWhiteSpace(SelectedYear))
            {
                int selectedYear = Convert.ToInt32(SelectedYear);
                query.AddOrAppendFilter(LogicalFilterOperator.And, $"CreatedYear IN ({selectedYear}, {selectedYear-1}, {selectedYear-2})");
            }
            Log("Search Criteria", query.Filter);
            return query;
        }

        #endregion

    }
}
