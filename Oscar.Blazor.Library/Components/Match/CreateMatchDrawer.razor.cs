using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Match
{
    public partial class CreateMatchDrawer
    {

        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public List<CountryDto> Countries { get; set; } = new();

        [Parameter]
        public List<EnumDTO> Rules { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public bool OpenMatchDrawer { get; set; } = false;

        [Parameter]
        public string? Title { get; set; } = string.Empty;

        [Parameter]
        public List<RightsTypeDto> RightsTypes { get; set; } = new();
    }
}
