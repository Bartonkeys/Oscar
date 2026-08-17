using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class ClientStatusPicker
    {
        [Parameter]
        public Expression<Func<Status?>>? For { get; set; }

        [Parameter]
        public Status? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<Status?> ValueChanged { get; set; }

        private async Task OnSelectChange(Status? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

    }

}
