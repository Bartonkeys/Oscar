using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Infrastructure.Features.Common.Contracts;
using static Oscar.Blazor.Library.Components.CopyForm;

namespace Oscar.Blazor.Pages
{
    public partial class Search
    {

        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private IBrowserDownload BrowserDownload { get; set; }

        private IEnumerable<WorksDto> _works;
        private string _searchString = string.Empty;
        private WorksDto _selectedItem;
        private HashSet<WorksDto> _selectedItems = new();
        private Discriminator _discriminator = Discriminator.All;
        private Discriminator _searchDiscriminator = Discriminator.All;
        private WorksStatus _statusDiscriminator = WorksStatus.Any;
        private int maxNumberOfRecordsForCsvExport = 10000;


        [Parameter]
        public int FilterClientID { get; set; }

        public int FilterCountryID { get; set; }
        public int FilterRightsCountryID { get; set; }

        private bool _loading = true;
        private bool openWorksDrawer;
        private bool openCopyDrawer;
        private bool openRightsDrawer;
        private int worksId;
        private string actionText = "";

        private IEnumerable<WorksDto> pagedData;
        private HashSet<WorksDto> selectedItems = new HashSet<WorksDto>();
        private MudTable<WorksDto> _table;
        private int RowsPerPageSession { get; set; }

        private int totalItems;
        private string searchString = null;

        private SearchType _searchTypeDiscriminator = SearchType.Exact;
        private SearchDirection _searchDirection = SearchDirection.Ascending;
        private WorksTitleResponseDto _selectedWorks = new(){Title = string.Empty};
        private bool _autoCompleteSearch;
        private MudAutocomplete<WorksTitleResponseDto> _autoComplete;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var result = await ProtectedSessionStore.GetAsync<int>("rowsPerPage");
                RowsPerPageSession = result.Success ? result.Value : 10;
            }
            else
            {
                if (_table != null && _table.RowsPerPage != RowsPerPageSession)
                {
                    _table.SetRowsPerPage(RowsPerPageSession);
                    StateHasChanged();
                }
            }

        }

        private async Task CopyFormComplete(CopyFormResponse response)
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
            searchString = "";
            _searchTypeDiscriminator = SearchType.Exact;
            _searchDirection = SearchDirection.Ascending;
            _selectedWorks = new() { Title = string.Empty };
            await _autoComplete.ClearAsync();
            StateHasChanged();

            await _table.ReloadServerData();
        }

        public async void onExportResults(EventArgs args)
        {
            await Task.Delay(1);

            _loading = true;

            IEntityTable<WorksDto> worksToExport;
            if (_autoCompleteSearch || string.IsNullOrEmpty(searchString))
            {
                //var works = (await Mediator.Send(new GetWorksByIdQuery { Id = _selectedWorks.WorksId })).Value;
                //worksTable = EntityTable<WorksDto>.Create(new List<WorksDto> { works }).WithTotal(1);

                worksToExport = (await Mediator.Send(new SearchByTitleQuery()
                {
                    Title = _selectedWorks.Title,
                    Start = 0,
                    Take = int.MaxValue,
                    SearchType = SearchType.Equals,
                    SearchDirection = _searchDirection
                })).Value;

                _autoCompleteSearch = false;
            }
            else
            {
                worksToExport = (await Mediator.Send(new SearchByTitleQuery()
                {
                    Title = searchString,
                    Start = 0,
                    Take = int.MaxValue,
                    SearchType = _searchTypeDiscriminator,
                    SearchDirection = _searchDirection
                })).Value;
            }


            if(worksToExport.TotalRecords > maxNumberOfRecordsForCsvExport)
            {
                Snackbar.Add("Too many records for export - please filter the results", Severity.Info);
                return;
            }
            else
            {
                Snackbar.Add("Export list being created for download - please do not navigate away from this page", Severity.Info);
            }
            var worksAsList = worksToExport.Records.ToList();

            await BrowserDownload.ExportWorksAsCsv(worksAsList, "WorksResultsExport.csv");

            _loading = false;

        }

        private static bool FilterBySearchString(WorksDto works, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(works.Titles.First().Title)
                   || works.Titles.First().Title.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<TableData<WorksDto>> ServerReload(TableState state, CancellationToken token)
        {
            await Task.Delay(1);

            _loading = true;

            _selectedItems = new();

            if ((string.IsNullOrEmpty(searchString) && !_autoCompleteSearch) || _selectedWorks == null)
            {
                _loading = false;
                return new TableData<WorksDto>() { TotalItems = 0, Items = new List<WorksDto>() };
            }

            IEntityTable<WorksDto> worksTable;

            if (_autoCompleteSearch)
            {
                worksTable = (await Mediator.Send(new SearchByTitleQuery()
                {
                    Title = _selectedWorks.Title,
                    Start = state.Page * state.PageSize,
                    Take = state.PageSize,
                    SearchType = SearchType.Equals,
                    SearchDirection = _searchDirection
                })).Value;

                _autoCompleteSearch = false;
            }
            else
            {
                worksTable = (await Mediator.Send(new SearchByTitleQuery()
                {
                    Title = searchString,
                    Start = state.Page * state.PageSize,
                    Take = state.PageSize,
                    SearchType = SearchType.Exact,
                    SearchDirection = _searchDirection,
                    Discriminator = _searchDiscriminator,
                    WorksStatus = _statusDiscriminator
                })).Value;
            }

            totalItems = worksTable.TotalRecords;
            pagedData = worksTable.Records.ToArray();

            _loading = false;
            return new TableData<WorksDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private async Task OnSearch()
        {
            _loading = true;
            await _table.ReloadServerData();
        }

        private void OpenWorksDrawer(int id, Discriminator discriminator)
        {
            openWorksDrawer = true;
            worksId = id;
            _discriminator = discriminator;
            StateHasChanged();
        }

        private async Task SearchTypeDiscriminator(IEnumerable<SearchType> arg)
        {
            _loading = true;
            _searchTypeDiscriminator = arg.First();
            await _table.ReloadServerData();
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
        }

        private async void UpdateRowsPerPageInSession(int newRowsPerPage)
        {
            await ProtectedSessionStore.SetAsync("rowsPerPage", newRowsPerPage);
            RowsPerPageSession = newRowsPerPage;
        }

        private async Task DirectionChanged(IEnumerable<SearchDirection> obj)
        {
            _loading = true;
            _searchDirection = obj.First();
            await _table.ReloadServerData();
        }

        private async Task<IEnumerable<WorksTitleResponseDto>> AutoCompleteTitle(string title, CancellationToken token)
        {
            if (string.IsNullOrEmpty(title)) 
                return new List<WorksTitleResponseDto>();

            var result = (await Mediator.Send(new GetWorksTitleAutoCompleteQuery
            {
                Title = title
            }));

            return result.Value;
        }

        private async Task GetWorks(WorksTitleResponseDto worksTitle)
        {
            _autoCompleteSearch = true;
            _selectedWorks = worksTitle;
            _loading = true;
            await _table.ReloadServerData();
        }

        private async Task Enter(KeyboardEventArgs e)
        {
            _loading = false;
            if (e.Code is "Enter" or "NumpadEnter")
            {
                _loading = true;
                await _table.ReloadServerData();
            }
        }

        private async Task FilterDiscriminator(IEnumerable<Discriminator> arg)
        {
            _searchDiscriminator = arg.First();
        }

        private async Task StatusFilterDiscriminator(IEnumerable<WorksStatus> arg)
        {
            _statusDiscriminator = arg.First();
        }
    }
}
