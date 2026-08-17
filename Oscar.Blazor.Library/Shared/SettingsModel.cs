using System;
using System.Runtime.CompilerServices;
using Oscar.Blazor.Library.Common;

namespace Oscar.Blazor.Library.Shared
{
    public class SettingsModel
    {
        private SpinnerType _spinnerType = SpinnerType.ChasingDots;
        public event Action OnChange;

        public bool ShowNotifications { get; set; }
        public bool ShowPageHints { get; set; } = true;
        public bool AuditInfo { get; set; }
        public bool AllowDelete { get; set; }
        
        public SpinnerType SelectedSpinnerStyle { get => _spinnerType; set => SetProperty(ref _spinnerType, value); }
        
        public void NotifyStateChanged() => OnChange?.Invoke();

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            var args = new PropertyChangedEventArgs { Name = propertyName, NewValue = value, OldValue = storage };
            storage = value;
            OnPropertyChanged(args);
            NotifyStateChanged();
            return true;
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            //Reload();
        }

    }
}
