using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Oscar.Core.Enums;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class StatusPicker
    {

        [Parameter]
        public Expression<Func<WorksStatus?>>? For { get; set; }

        [Parameter]
        public WorksStatus? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public List<WorksStatus> InvalidStatuses { get; set; } = new List<WorksStatus>();

        [Parameter]
        public EventCallback<WorksStatus?> StatusChanged { get; set; }

        private async Task OnSelectChange(WorksStatus? newValue)
        {
            Value = newValue;
            await StatusChanged.InvokeAsync(Value);
        }

    }
}
