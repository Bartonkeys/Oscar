using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components
{
    public partial class LanguagePicker
    {
        private List<LanguageDto> _languages;

        [Parameter]
        public Expression<Func<string>> For { get; set; }

        [Parameter]
        public String LanguageCode { get; set; } = "";

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        protected override async void OnParametersSet()
        {
            _languages = (await Mediator.Send(new GetLanguageStaticDataQuery { })).Value;
            StateHasChanged();
        }
    }
}
