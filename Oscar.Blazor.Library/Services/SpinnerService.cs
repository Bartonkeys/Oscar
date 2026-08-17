namespace Oscar.Blazor.Library.Services
{
    public class SpinnerService
    {
        public event Action OnShow;
        public event Action OnHide;
        public event Action OnStyleChange;

        public void Show()
        {
            OnShow?.Invoke();
        }
        public void Hide()
        {
            OnHide?.Invoke();
        }
        public void OnChange()
        {
            OnStyleChange?.Invoke();
        }
    }
    public class SpinnerHandler : DelegatingHandler
    {
        private readonly SpinnerService _spinnerService;

        public SpinnerHandler(SpinnerService spinnerService)
        {
            _spinnerService = spinnerService;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _spinnerService.Show();
            var response = await base.SendAsync(request, cancellationToken);
            _spinnerService.Hide();
            return response;
        }
    }
}
