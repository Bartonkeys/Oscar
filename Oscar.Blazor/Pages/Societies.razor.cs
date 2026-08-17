using Oscar.Core.DTOs;
using MudBlazor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Oscar.Blazor.Components;

namespace Oscar.Blazor.Pages
{
    public partial class Societies
    {
        protected EventConsole Console;
        private IEnumerable<SocietyDto> _societies;
        private int societyId;
        private string _searchString = null;
        private SocietyDto _selectedItem;
        private bool openSocietyDrawer;
        private bool openRightsDrawer;
        private bool _updatingPageSize = false;
        private int RowsPerPageSession { get; set; }
        public MudTable<SocietyDto>? _table;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadStateAsync();
                SetPageSizeFromSessionState();
                await LoadSocieties();
            }
            else
            {
                if (!_updatingPageSize)
                {
                    _updatingPageSize = true;
                    SetPageSizeFromSessionState();
                    _updatingPageSize = false;
                }
            }
        }

        private async Task LoadStateAsync()
        {
            var result = new ProtectedBrowserStorageResult<int>();

            try
            {
                result = await ProtectedLocalStore.GetAsync<int>("rowsPerPage");
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
            }

            RowsPerPageSession = result.Success ? result.Value : 10;
        }

        private void SetPageSizeFromSessionState()
        {
            if (RowsPerPageSession == 0) RowsPerPageSession = 10;
            if (_table != null && _table.RowsPerPage != RowsPerPageSession)
            {
                _table.SetRowsPerPage(RowsPerPageSession);
                StateHasChanged();
            }
        }

        private async Task SaveStateAsync(int newRowsPerPage)
        {
            RowsPerPageSession = newRowsPerPage;
            try
            {
                await ProtectedLocalStore.SetAsync("rowsPerPage", newRowsPerPage);
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
            }
        }
        private async Task LoadSocieties()
        {
            try
            {
                await SetStatusAsync(true, "Loading Societies");
                _societies = await RefDataService.GetSocieties();
            }
            finally
            {
                await SetStatusAsync(false, "Societies Loaded");
            }
        }

        private bool Filter(SocietyDto society) => FilterBySearchString(society, _searchString);

        private static bool FilterBySearchString(SocietyDto society, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(society.Name)
                   || society.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }

        private void OpenSocietyForm(int id)
        {
            openSocietyDrawer = true;
            societyId = id;
        }

        private void OpenRightsForm(int id)
        {
            openRightsDrawer = true;
            societyId = id;
        }

        protected async Task SuccessCallback()
        {
            openSocietyDrawer = false;
            await LoadSocieties();
        }

        protected async Task SuccessRightsCallback()
        {
            openRightsDrawer = false;
            await LoadSocieties();
        }

        private async void UpdateRowsPerPageInSession(int newRowsPerPage)
        {
            _updatingPageSize = true;

            await SaveStateAsync(newRowsPerPage);
            StateHasChanged();
            _updatingPageSize = false;
        }
    }
}
