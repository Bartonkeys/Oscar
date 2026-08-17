using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.Common;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class Contract
    {
        private DateTime? _endOfValidityHolder = DateTime.Now;

        [Parameter] 
        public ClientDto? Value { get; set; }

        [Parameter]
        public EventCallback<ClientDto> ValueChanged { get; set; }

        [Parameter]
        [Category("Validation")]
        public Expression<Func<ClientDto>>? For { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if(Value.Contract == null)
            {
                Value.Contract = new();
                Value.Contract.EndDate = Constants.Rights.Perpetuity;
            }
        }

        private Variant GetVariant(bool isSelected)
        {
            return isSelected ? Variant.Filled : Variant.Outlined;
        }

        private void ToggleEndDatePerpetuity(bool toggled)
        {
            if (toggled)
            {
                _endOfValidityHolder = Value.Contract.EndDate;
                Value.Contract.EndDate = Constants.Rights.Perpetuity;
            }
            else
            {
                Value.Contract.EndDate = _endOfValidityHolder;
            }
        }

    }
}

