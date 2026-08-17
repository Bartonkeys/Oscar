using Microsoft.AspNetCore.Components;
using Oscar.Blazor.Library.Services;

namespace Oscar.Blazor.Library.Shared
{
    public partial class Spinner : ComponentBase
    {
        [Inject] protected SpinnerService SpinnerService { get; set; }
        [Inject] protected SettingsModel SettingModel { get; set; }
        private SpinnerType _spinnerStyle = SpinnerType.Cube;
        protected bool IsVisible { get; set; }

        [Parameter]
        public SpinnerType SpinnerStyle { get => _spinnerStyle; set { _spinnerStyle = value; StateHasChanged(); } }

        [Parameter]
        public string SpinnerText { get; set; } = "Please Wait...";
        protected override void OnInitialized()
        {
            ReloadSettings();
            SpinnerService.OnShow += ShowSpinner;
            SpinnerService.OnHide += HideSpinner;
            SpinnerService.OnStyleChange += ReloadSettings;
        }
        public void ShowSpinner()
        {
            IsVisible = true;
            InvokeAsync(StateHasChanged);
        }

        public void HideSpinner()
        {
            IsVisible = false;
            InvokeAsync(StateHasChanged);
        }

        protected void ReloadSettings()
        {
            SettingModel ??= new SettingsModel();
            SpinnerStyle = SettingModel.SelectedSpinnerStyle;
            InvokeAsync(StateHasChanged);
        }
    }
}
