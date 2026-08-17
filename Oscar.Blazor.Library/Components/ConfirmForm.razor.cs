using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;

namespace Oscar.Blazor.Library.Components
{
    public partial class ConfirmForm
    {
        private MudTable<WorksDto> table;

        protected override void OnInitialized()
        {
            //
        }

        [Parameter]
        public HashSet<WorksDto>? WorksToCopy { get; set; }

        [Parameter]
        public ClientDto? ClientCopiedTo { get; set; }
        
        [Parameter]
        public CatalogueDto? CatalogueCopiedTo { get; set; }

        [Parameter]
        public string ActionText { get; set; }

        protected override async void OnParametersSet()
        {
            StateHasChanged();
        }
    }
}