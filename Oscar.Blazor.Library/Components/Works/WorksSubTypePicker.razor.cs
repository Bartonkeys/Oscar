using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class WorksSubTypePicker
    {
        private List<WorksSubTypeDto> _types;

        [Parameter]
        public Expression<Func<int?>>? For { get; set; }

        [Parameter]
        public int? Value { get; set; } = 0;

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<int?> ValueChanged { get; set; }

        private async Task OnSelectChange(int? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        protected override async Task OnInitializedAsync()
        {
            _types = (await Mediator.Send(new GetWorksSubTypeStaticDataQuery { })).Value;
            StateHasChanged();
        }
    }
}
