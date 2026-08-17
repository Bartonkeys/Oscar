using System.Linq.Expressions;
using System.Windows.Markup;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Clients
{

    public partial class OtherNames
    {
        [Parameter]
        public Expression<Func<ICollection<OtherNameDto>>> For { get; set; }

        [Parameter]
        public ICollection<OtherNameDto?>? Value { get; set; }

        private bool _loaded = false;


        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public int ClientId { get; set; } = -1;

        [Parameter]
        public int CatalogueId { get; set; } = -1;

        [Parameter]
        public EventCallback<ICollection<OtherNameDto>> ValueChanged { get; set; }



        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            _loaded = true;
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _loaded = false;

            if (Value == null)
            {
                Value = new List<OtherNameDto>();
                OtherNameDto newOtherName = new();
                newOtherName.Type = OtherNameType.AKA;
                Value.Add(newOtherName);
                await ValueChanged.InvokeAsync(Value);
            }

            StateHasChanged();
        }

        private async void addOtherName()
        {
            int uid = -1;

            if (Value.Any())
            {
                uid = Value.Min(i => i.Id);

                if (uid > -1)
                    uid = 0;

                uid--;
            }

            Value.Add(new OtherNameDto {
                Type = OtherNameType.AKA,
                Name = string.Empty,
                Id = uid,
                ClientId = ClientId,
                CatalogueId = CatalogueId
            });
            
            await ValueChanged.InvokeAsync(Value);

            StateHasChanged();
        }

        private async void removeOtherName(OtherNameDto otherName)
        { 
            var otherNameDto = Value.Single(v => v.Id == otherName.Id);
            Value.Remove(otherNameDto);
                
            await ValueChanged.InvokeAsync(Value);

            StateHasChanged();
        }
        
    }
}
