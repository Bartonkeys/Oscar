using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class ClientReferences
    {
        [Parameter]
        public ClientDto Client { get; set; }
    }
}
