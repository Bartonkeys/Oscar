using System.IO.Pipes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Services;
using Oscar.Blazor.Components;
using MudBlazor;
using Oscar.Blazor.Library.Components;
using Microsoft.JSInterop;
using BartonKeys.Functional;
using Microsoft.AspNetCore.Html;

namespace Oscar.Blazor.Pages.Reports
{
    public abstract class OscarReportComponent: OscarComponentBase
    {
        #region - DI -
        [Inject]
        protected OscarDataService ReportService { get; set; }
        #endregion

        #region - Properties -

        public bool ShowInfoBanner { get; set; }
        public string ReportName { get; set; }
        public string ReportTitle { get; set; }
        protected EventConsoleComponent Console { get; set; }
        #endregion

        #region - Virtual Methods -
        public virtual void InitReportDefaults() { }

        public virtual async Task ResetSearchCriteria() { await Task.CompletedTask; }
        public virtual async Task ExecuteReportSearch() { await Task.CompletedTask; }

        public abstract Task<FileStreamResult> ExportReportToExcel();
        public abstract Task<FileStreamResult> ExportReportToCsv();

        protected virtual async Task<bool> IsUserConfirmed(string operation)
        {
            return await Task.FromResult(true);
        }

        protected virtual string Validate() { return string.Empty; }
        #endregion

        public async Task OnResetClick()
        {
            try
            {
                await SetStatusAsync(true, "Resetting");
                await ResetSearchCriteria();
            }
            finally
            {
                await SetStatusAsync(false, "Reset Successful");
            }
        }

        public async Task OnRunReportClick()
        {
            if (!await IsValidated()) return;
            if (!await IsUserConfirmed("Searching")) return;
            await SetStatusAsync(true, "Loading Report");
            Log( "RunReport", $"Executing {ReportName}");
            try
            {
                await ExecuteReportSearch();
            }
            catch (Exception ex)
            {
                LogException(ex);
                Snackbar.Add($"Unable search {ReportTitle}. Error: {ErrorMessage}", Severity.Error);
            }
            finally
            {
                await SetStatusAsync(false, "Execution Completed");
                Log("RunReport", StatusText);
            }
        }

        public async Task OnExportToExcelClick()
        {
            if (!await IsValidated()) return;
            if (!await IsUserConfirmed("Exporting to excel")) return;
            await SetStatusAsync(true, "Preparing Report");
            try
            {
                var result = await ExportReportToExcel();
                await DownloadFromFileStream(result);
                Snackbar.Add($"{ReportTitle} export ready to download.", Severity.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                Snackbar.Add($"Unable to download {ReportTitle}. Error: {ErrorMessage}", Severity.Error);
            }
            finally
            {
                await SetStatusAsync(false, "Export Completed");
                Log("Export to Excel", StatusText);
            }
        }

        public async Task OnExportToCsvClick()
        {
            if (!await IsValidated()) return;
            if (!await IsUserConfirmed("Exporting to Csv")) return;
            await SetStatusAsync(true, "Preparing Report");
            try
            {
                var result = await ExportReportToCsv();
                await DownloadFromFileStream(result);
                Snackbar.Add($"{ReportTitle} export ready to download.", Severity.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                Snackbar.Add($"Unable to download {ReportTitle}. Error: {ErrorMessage}", Severity.Error);
            }
            finally
            {
                await SetStatusAsync(false, "Export Completed");
                Log("Export to CSV", StatusText);
            }
        }

        protected void CloseInfoBanner(bool value)
        {
            ShowInfoBanner = !value;
            StateHasChanged();
        }

        protected void ToggleInfoBanner()
        {
            ShowInfoBanner = !ShowInfoBanner;
            StateHasChanged();
        }
        #region - Protected Methods -

        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitReportDefaults();
        }

        protected async Task<bool> GetUserConfirmation(string message, string title = "Warning", string yesText = "Continue", string cancelText = "Cancel")
        {
            var htmlMessage = (MarkupString)message;
            var options = new MessageBoxOptions
            {
                Title = title,
                MarkupMessage = htmlMessage,
                CancelText = cancelText, YesText = yesText
            };
            bool? result = await DialogService.ShowMessageBoxAsync(options);
            Log("User Confirmation", $"Message: {message}, Confirmed: {result.GetValueOrDefault()}");
            return result.GetValueOrDefault();
        }

        protected async Task<bool> IsValidated()
        {
            var message = Validate();
            if (string.IsNullOrWhiteSpace(message)) return true;
            var htmlMessage = (MarkupString)$"{message}<br/>Please provide mandatory fields and try again.";
            MessageBoxOptions options = new MessageBoxOptions() { MarkupMessage = htmlMessage, Title = "Validation", };
            await DialogService.ShowMessageBoxAsync(options);
            return false;
        }

        private async Task DownloadFromFileStream(FileStreamResult response)
        {
            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", response.FileDownloadName, new DotNetStreamReference(stream: response.FileStream));
        }

        protected void Log(string eventName, string value)
        {
            Console?.Log($"{eventName}: {value}");
        }
        protected void LogInfo(string text)
        {
            Console?.Log(text);
        }
        protected void LogError(string text)
        {
            Console?.LogError(text);
        }
        protected void LogException(Exception ex)
        {
            Console?.LogException(ex);
        }
        #endregion
    }
}
