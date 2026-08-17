using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oscar.Blazor.Library.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Commands;
using BartonKeys.Functional;
using Oscar.Core.Enums;

namespace Oscar.Blazor.Pages
{
    public partial class ReportSearch
    {
        private IEnumerable<ReportDto> reports;
        private string _searchString = string.Empty;
        private ReportDto _selectedItem;
        private HashSet<ReportDto> _selectedItems = new();
        private bool _loading = true;

        private IEnumerable<ReportDto> pagedData;
        private MudTable<ReportDto> table;

        private int totalItems;
        private string searchString = null;

        private bool Filter(ReportDto report) => FilterBySearchString(report, _searchString);

        private static bool FilterBySearchString(ReportDto reports, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(reports.ReportName)
                   || reports.ReportName.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<TableData<ReportDto>> ServerReload(TableState state)
        {

            table.Loading = true;

            var getReportsQuery = new GetReportsQuery();
            if (searchString != null)
            {
                var SearchObjects = new List<SearchObject>();
                SearchObjects.Add(new SearchObject("Report", "string", "ReportName", searchString));
                getReportsQuery.SearchObjects = SearchObjects;
            }
            if (state.SortLabel != null)
            {
                getReportsQuery.SortColumn = state.SortLabel;
                getReportsQuery.SortDirection = state.SortDirection == SortDirection.Descending ? "descending" : "ascending";
            }

            var reportsTable = (await Mediator.Send(getReportsQuery)).Value;

            totalItems = reportsTable.TotalRecords;
            pagedData = reportsTable.Records.ToArray();
            _loading = false;
            return new TableData<ReportDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        private async void DeleteReport(ReportDto reportDto)
        {
            var dialog = DialogService.Show<ConfirmDialog>("Delete Report?");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var result = await Mediator.Send(new DeleteReportCommand { Id = reportDto.Id });
                if (result.IsSuccess)
                    Snackbar.Add("Report deleted", Severity.Success);
                else
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });

                await table.ReloadServerData();
            }
        }

        private async void EditReport(ReportDto reportDto)
        {
            var ReportId = reportDto.Id;
            NavigationManager.NavigateTo("reportCreate" + "/" + ReportId);
        }

        private async void RunReport(ReportDto reportDto)
        {
            var ReportId = reportDto.Id;
            NavigationManager.NavigateTo($"ReportRun/{reportDto.Id}");
        }

        private void OpenCreateReportForm()
        {
            NavigationManager.NavigateTo($"reportCreate");
        }

        private Color GetColour(ReportStatus contextReportStatus)
        {
            switch (contextReportStatus)
            {
                case ReportStatus.Ready:
                    return Color.Success;
                case ReportStatus.Error:
                    return Color.Error;
                case ReportStatus.Queued:
                    return Color.Secondary;
                case ReportStatus.Building:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
