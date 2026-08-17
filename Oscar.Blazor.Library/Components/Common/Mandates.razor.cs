using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class Mandates
    {
        [Parameter]
        public IEnumerable<MandateTypeDto> MandateTypes { get; set; }
    }
}

