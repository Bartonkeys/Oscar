using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components
{
    public partial class DuplicateDialog
    {
        [CascadingParameter]
        IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public ICollection<DuplicateDto>? Duplicates { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Ok()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

    }
}