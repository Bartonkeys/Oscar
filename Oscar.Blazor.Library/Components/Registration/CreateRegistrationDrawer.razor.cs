using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Registration
{
    public partial class CreateRegistrationDrawer
    {
        private CreateRegistrationForm _createRegistrationForm;

        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public List<SocietyDto> Societies { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public bool OpenRegistrationDrawer { get; set; } = false;

        [Parameter]
        public string? Title { get; set; } = string.Empty;


    }
}
