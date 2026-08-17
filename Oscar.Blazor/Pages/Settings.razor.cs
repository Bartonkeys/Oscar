using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Shared;

namespace Oscar.Blazor.Pages
{
    public partial class Settings: OscarComponentBase
    {
        protected SpinnerType _selectedSpinner;

        public SpinnerType SelectedSpinner
        {
            get => _selectedSpinner;
            set
            {
                SetProperty(ref _selectedSpinner, value);
                SettingModel.SelectedSpinnerStyle = value;
                SimulateLongProcess();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _selectedSpinner = SettingModel.SelectedSpinnerStyle;
        }

        private void SimulateLongProcess()
        {
            Task.Run(async () =>
            {
                try
                {
                    await SetStatusAsync(true, "Simulating");
                    await Task.Delay(2000);
                }
                finally
                {
                    await SetStatusAsync(false, "Complete");
                }
            });
        }
    }
}
