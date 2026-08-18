using MudBlazor;
using Oscar.Blazor.Components;
using Oscar.Core.Common;
using Oscar.Core.DTOs;
using Oscar.Blazor.Library.Common;
using Microsoft.AspNetCore.Components.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;

namespace Oscar.Blazor.Pages
{
    public partial class Clients : OscarComponentBase
    {
        #region - Private Declarations -

        protected EventConsole Console { get; set; }

        private IEnumerable<ClientDto> _clients;
        private List<RightsTypeDto> _rightsType;
        private List<CountryDto> _countries;
        private ClientDto _selectedItem;
        public MudDataGrid<ClientDto>? _dataGrid = null;

        private int _clientId;
        private bool _openClientDrawer;
        private bool _openRightsDrawer;

        private bool _hideAltName = true;
        private bool _hideEmail = true;
        private bool _hideContractFirstStartDate = true;
        private bool _hideContractCurrentStartDate = true;
        private bool _hideContractEndDate = true;
        private bool _hideLastModified = true;
        private bool _hideCreatedDate = true;
        private bool _hideIMaestroClientCode = true;
        private bool _stickClientName = false;
        private bool _stickActions = false;
        private bool _loadingSession = false;
        private bool _settingsModified = false;
        private bool _fixedHeader = true;
        private bool _fixedFooter = true;

        #endregion

        #region - Properties -

        public int FilterClientID { get; set; }

        public int ClientId
        {
            get => _clientId;
            set => SetPropertyEx(ref _clientId, value);
        }

        public bool HideAltName
        {
            get => _hideAltName;
            set
            {
                if (SetProperty(ref _hideAltName, value)) OnColumnStateChanged(value);
            }
        }

        public bool HideEmail
        {
            get => _hideEmail;
            set
            {
                if (SetProperty(ref _hideEmail, value)) OnColumnStateChanged(value);
            }
        }

        public bool HideContractFirstStartDate
        {
            get => _hideContractFirstStartDate;
            set
            {
                if (SetProperty(ref _hideContractFirstStartDate, value)) OnColumnStateChanged(value);
            }
        }

        public bool HideContractCurrentStartDate
        {
            get => _hideContractCurrentStartDate;
            set
            {
                if (SetProperty(ref _hideContractCurrentStartDate, value)) OnColumnStateChanged(value);
            }
        }

        public bool HideContractEndDate
        {
            get => _hideContractEndDate;
            set
            {
                if (SetProperty(ref _hideContractEndDate, value)) OnColumnStateChanged(value);
            }

        }

        public bool HideLastModified
        {
            get => _hideLastModified;
            set
            {
                if (SetProperty(ref _hideLastModified, value)) OnColumnStateChanged(value);
            }
        }

        public bool HideCreatedDate
        {
            get => _hideCreatedDate;
            set
            {
                if (SetProperty(ref _hideCreatedDate, value)) OnColumnStateChanged(value);
            }
        }
        public bool HideIMaestroClientCode
        {
            get => _hideIMaestroClientCode;
            set
            {
                if (SetProperty(ref _hideIMaestroClientCode, value)) OnColumnStateChanged(value);
            }
        }

        public bool StickClientName
        {
            get => _stickClientName;
            set { if (SetPropertyEx(ref _stickClientName, value)) SettingsModified = true; }
        }

        public bool StickActions
        {
            get => _stickActions;
            set { if (SetPropertyEx(ref _stickActions, value)) SettingsModified = true; }
        }

        public bool SettingsModified
        {
            get => _settingsModified;
            set
            {
                _settingsModified = value;
                if (value) SaveStateAsync().RunSynchronously();
            }
        }
        public ClientDto SelectedItem { get => _selectedItem; set => SetPropertyEx(ref _selectedItem, value); }

        #endregion

        #region - Method Overrides -
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task OnPageSizeChangedAsync(int newPageSize)
        {
            if (newPageSize == RowsPerPageSession) return;
            RowsPerPageSession = newPageSize;
            await base.SaveStateAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadClients();
                await LoadStateAsync();
            }
        }
        #endregion

        #region - Protected Methods -
        protected async Task SuccessCallback(int newClientId)
        {
            if (newClientId < 0)
            {
                _openClientDrawer = false;
                Console?.Log("Edit Canceled");
            }
            else
            {
                Console?.Log("Client Changed", $"ClientId = {newClientId}");
                await ReloadClients();
                if (newClientId > 0)
                {
                    OpenClientForm(newClientId);
                }
            }
        }

        protected async Task SuccessRightsCallback()
        {
            _openRightsDrawer = false;
            Console?.Log("Closed Rights");
            await ReloadClients();
        }

        #endregion

        #region - Private Methods -

        protected override async Task LoadStateAsync()
        {
            if (_loadingSession) return;
            try
            {
                _loadingSession = true;
                if (_dataGrid != null)
                {
                    _dataGrid.PagerStateHasChangedEvent += async () => await OnPageSizeChangedAsync(_dataGrid.RowsPerPage);
                }
                await base.LoadStateAsync();
                SetPageSizeFromSessionState();

                //var settings = await ProtectedLocalStore.GetAsync<bool>("StickClientName");
                //if (settings.Success) { StickClientName = settings.Value; }

                //settings = await ProtectedLocalStore.GetAsync<bool>("StickActions");
                //if (settings.Success) { StickActions = settings.Value; }

                var resultColumnsSession = await ProtectedLocalStore.GetAsync<IEnumerable<string>>("ClientListColumn");
                if (resultColumnsSession.Success && resultColumnsSession.Value != null)
                {
                    SetColumnsVisible(resultColumnsSession.Value);
                }

                Console?.Log("Session State Loaded");
                _loadingSession = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
                _loadingSession = false;
            }
        }

        protected override async Task SaveStateAsync()
        {
            RowsPerPageSession = _dataGrid?.RowsPerPage ?? 25;
            //await ProtectedLocalStore.SetAsync("StickClientName", StickClientName);
            //await ProtectedLocalStore.SetAsync("StickActions", StickActions);
            await ProtectedLocalStore.SetAsync("ClientListColumn", GetColumnsVisible());

            await base.SaveStateAsync();
        }
        protected override void SetPageSizeFromSessionState()
        {
            if (_dataGrid != null && _dataGrid.RowsPerPage != RowsPerPageSession)
            {
                _dataGrid.SetRowsPerPageAsync(RowsPerPageSession);
            }
        }

        private async void SaveSettings(MouseEventArgs e)
        {
            try
            {
                await SaveStateAsync();
                Console?.Log("Session State Saved");
                Snackbar.Add("Session State Saved.", Severity.Success);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
            }
        }

        private async Task HideColumnsAsync(bool hide)
        {
            if (_dataGrid == null) return;
            foreach (var column in _dataGrid.RenderedColumns)
            {
                if (hide)
                {
                    await column.HideAsync();
                }
                else
                {
                    await column.ShowAsync();
                }
            };
        }

        private async Task LoadClients()
        {
            await SetStatusAsync(true, "Loading Clients");
            try
            {
                _clients = await RefDataService.GetClients();
                _rightsType = await RefDataService.GetRightsType();
                _countries = await RefDataService.GetCountries();
            }
            finally
            {
                await SetStatusAsync(false, "Clients Loaded");
            }
        }

        private async Task ReloadClients()
        {
            await RefDataService.LoadClientsAsync(true);
            await RefDataService.LoadRightsTypeAsync(true);
            await RefDataService.LoadCountriesAsync(true);
            await LoadClients();
        }

        private void LinkWork(int id)
        {
            Console?.Log($"Opening Works for Client: {id}");
            FilterClientID = id;
            NavigationManager.NavigateTo("works" + "/" + FilterClientID);
        }

        private bool ClientFilter(ClientDto client)
        {
            if (client == null || string.IsNullOrWhiteSpace(SearchText)) return true;
            return client.FilterBy(SearchText);
        }

        private void OpenClientForm(int id)
        {
            _openClientDrawer = true;
            ClientId = id;
            Console?.Log($"Opening Client: {id}");
        }

        private void OpenRightsForm(int id)
        {
            _openRightsDrawer = true;
            ClientId = id;
            Console?.Log($"Opening Rights for Client: {id}");
        }

        private static string GetFormattedDate(DateTime? date)
        {
            if (date != null)
            {
                if (date == Constants.Rights.Perpetuity)
                {
                    return "Perpetuity";
                }
                return Convert.ToDateTime(date).ToString("dd/MM/yyyy");
            }
            return "";
        }

        private List<string> GetColumnsVisible()
        {
            return _dataGrid != null ? _dataGrid.RenderedColumns.Where(x => !x.Hidden).Select(x => x.Title).ToList() : new List<string>();
        }

        private async void SetColumnsVisible(IEnumerable<string> columns)
        {
            if (columns == null || !columns.Any()) return;
            if (_dataGrid == null) return;

            foreach (var column in _dataGrid.RenderedColumns)
            {
                if (columns.Contains(column.Title))
                    await column.ShowAsync();
            }
        }

        private async void OnColumnStateChanged(bool isVisible)
        {
            await SaveStateAsync();
        }

        #endregion

    }
}
