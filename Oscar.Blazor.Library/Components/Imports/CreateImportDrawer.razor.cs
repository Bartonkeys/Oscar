using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Imports
{
    public partial class CreateImportDrawer
    {

        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public bool OpenImportDrawer { get; set; } = false;

        [Parameter]
        public bool ImportWorks { get; set; } = true;

        [Parameter]
        public string? Title { get; set; } = string.Empty;

    }
}
