using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Blazor.Library.Shared;
using Oscar.Infrastructure.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Oscar.Blazor.Library.Common
{
    public class OscarComponentBase : ComponentBase, IDisposable
    {

        #region - Injected Services -
        [Inject]
        protected ISnackbar Snackbar { get; set; }

        [Inject]
        protected IMediator Mediator { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IDialogService DialogService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        protected ReferenceDataService RefDataService { get; set; }

        [Inject]
        protected SettingsModel SettingModel { get; set; }

        [Inject]
        protected ProtectedLocalStorage ProtectedLocalStore { get; set; }
        #endregion

        #region - Protected Members -

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            var args = new PropertyChangedEventArgs { Name = propertyName, NewValue = value, OldValue = storage };
            storage = value;
            OnPropertyChanged(args);
            return true;
        }
        /// <summary>
        /// Extension of SetProperty with StateHasChanged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetPropertyEx<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = SetProperty(ref storage, value, propertyName);
            StateHasChanged();
            return result;
        }
        #endregion

        #region - Private Declarations -
        private Stopwatch _stopwatch = new Stopwatch();
        private bool _isBusy;

        private bool _isDatabaseConnected = false;
        private string _errorMessage;
        private string _searchText;
        private string _statusText;
        private int[] _pageSizeOptions = new int[] { 10, 25, 50, 100 };

        private int _rowsPerPageSession = 10;
        #endregion

        #region - Public Properties -
        protected Spinner SpinnerControl { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        protected bool HasFaulted => !string.IsNullOrWhiteSpace(_errorMessage);

        protected string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        protected string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        protected int[] PageSizeOptions
        {
            get => _pageSizeOptions;
            set => SetProperty(ref _pageSizeOptions, value);
        }

        public string StatusText
        {
            get => _statusText;
            protected set => SetProperty(ref _statusText, value);
        }

        public int RowsPerPageSession
        {
            get => _rowsPerPageSession;
            protected set
            {
                if (SetProperty(ref _rowsPerPageSession, value == 0 ? 25 : value))
                {
                    ProtectedLocalStore.SetAsync("rowsPerPage", _rowsPerPageSession);
                    SetPageSizeFromSessionState();
                }
            }
        }
        #endregion

        #region - Protected Methods -

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SettingModel ??= new SettingsModel();
        }

        protected override async Task OnInitializedAsync()
        {
            await Load();
        }

        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {

        }

        protected virtual async Task Load()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                SearchText = "";
            }

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void SetSpinner(bool show)
        {
            if (show)
                SpinnerControl?.ShowSpinner();
            else
                SpinnerControl?.HideSpinner();
        }

        public void SetStatus(bool isBusy, string statusText = null)
        {
            var status = !string.IsNullOrWhiteSpace(statusText) ? statusText : (isBusy ? "Loading" : "Loaded");

            if (isBusy)
            {
                if (isBusy != IsBusy) { _stopwatch.Reset(); _stopwatch.Start(); }
                StatusText = $"{status}, Please Wait...";
                IsBusy = true;
            }
            else
            {
                _stopwatch.Stop();
                StatusText = $"{status} in {(_stopwatch.Elapsed.ToDuration())}";
                IsBusy = false;
            }
            SetSpinner(IsBusy);
            InvokeAsync(StateHasChanged);
        }

        public async Task SetStatusAsync(bool isBusy, string statusText = null)
        {
            SetStatus(isBusy, statusText);
            if (isBusy) { await Task.Delay(2); } //hack: 2ms delay to get spinner to start
            await Task.CompletedTask;
        }

        #endregion

        #region - Internal Methods -
        internal async Task HandleInvalidSubmit()
        {
            await JSRuntime.InvokeVoidAsync("scrollToFirstError");
        }
        #endregion

        protected async Task UpdateRowsPerPageInSession(int newRowsPerPage)
        {
            RowsPerPageSession = newRowsPerPage;
            await SaveStateAsync();
        }

        protected virtual void SetPageSizeFromSessionState() { }

        protected virtual async Task SaveStateAsync()
        {
            await ProtectedLocalStore.SetAsync("rowsPerPage", RowsPerPageSession);
        }

        protected virtual async Task LoadStateAsync()
        {
            var resultPageSession = await ProtectedLocalStore.GetAsync<int>("rowsPerPage");
            RowsPerPageSession = resultPageSession.Success ? resultPageSession.Value : 25;
        }
    }

    public class PropertyChangedEventArgs
    {
        public string Name { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }
}
