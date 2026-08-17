using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Oscar.Core.Enums;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class CommissionedWorkStatusPicker
    {

        [Parameter]
        public Expression<Func<CommissionedWorkStatus?>>? For { get; set; }

        [Parameter]
        public CommissionedWorkStatus? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public List<CommissionedWorkStatus> InvalidStatuses { get; set; } = new List<CommissionedWorkStatus>();

        [Parameter]
        public EventCallback<CommissionedWorkStatus?> ValueChanged { get; set; }

        private async Task OnSelectChange(CommissionedWorkStatus? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

    }
}
