using Microsoft.AspNetCore.Components;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class WorksDrawer
    {
        [Parameter]
        public bool OpenDrawer { get; set; } = false;

        [Parameter]
        public string? Title { get; set; } = string.Empty;

        protected override void OnParametersSet()
        {
            Snackbar.Add($"Open: {OpenDrawer} Title: {Title}");
        }
    }
}
