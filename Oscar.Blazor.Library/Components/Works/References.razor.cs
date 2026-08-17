using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class References
    {
        [Parameter]
        public WorksDto Works { get; set; }

        private async Task SocietyReferencesChanged(ICollection<SocietyReferenceDto> SocietyReferenceDtos)
        {
            Works.SocietyReferences = SocietyReferenceDtos;
        }

    }
}
