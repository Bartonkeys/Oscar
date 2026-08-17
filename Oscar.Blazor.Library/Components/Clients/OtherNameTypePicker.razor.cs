using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oscar.Core.Enums;


namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class OtherNameTypePicker
    {
        [Parameter]
        public Expression<Func<OtherNameType?>>? For { get; set; }

        [Parameter]
        public OtherNameType? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<OtherNameType?> ValueChanged { get; set; }

        private async Task OnSelectChange(OtherNameType? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

    }

}
