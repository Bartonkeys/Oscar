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
    public partial class ClientGradePicker
    {
        [Parameter]
        public Expression<Func<ClientGrade?>>? For { get; set; }

        [Parameter]
        public ClientGrade? Value { get; set; }

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ClientGrade?> ValueChanged { get; set; }

        private async Task OnSelectChange(ClientGrade? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

    }

}
