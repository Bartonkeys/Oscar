using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class WorksTypePicker
    {
        private List<WorksTypeDto> _types;
        private MudSelect<int?> _worksTypePicker;

        [Parameter]
        public Expression<Func<int?>>? For { get; set; }

        [Parameter]
        public int? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public bool ReadOnly { get; set; } = false;

        [Parameter]
        public string DefaultValue { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<int?> ValueChanged { get; set; }

        private async Task OnSelectChange(int? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        protected override async Task OnInitializedAsync()
        {
            _types = (await Mediator.Send(new GetTypeStaticDataQuery { })).Value;

            if (DefaultValue != string.Empty)
            {
                _types = _types?.Where(x => x.Name == DefaultValue).ToList();
                Value = _types?.FirstOrDefault(x => x.Name == DefaultValue).Id;
                await ValueChanged.InvokeAsync(Value);
            }
            StateHasChanged();
        }

        public async Task Clear()
        {
            Value = null;
            await _worksTypePicker.Clear();
        }

    }
}

