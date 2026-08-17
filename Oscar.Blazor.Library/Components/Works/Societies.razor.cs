using System.Linq.Expressions;
using System.Windows.Markup;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Societies
    {
        [Parameter]
        public Expression<Func<ICollection<SocietyReferenceDto>>> For { get; set; }

        [Parameter]
        public ICollection<SocietyReferenceDto> Value { get; set; }

        private List<SocietyReferenceDto> _societies;

        [Parameter]
        public EventCallback<ICollection<SocietyReferenceDto>> ValueChanged { get; set; }

        public async void changeSociety(SocietyReferenceDto newSociety, SocietyReferenceDto oldSocRef)
        {
            SocietyReferenceDto obj = Value.FirstOrDefault(x => x.Id == oldSocRef.Id);
            if (obj != null)
            {
                obj.SocietyId = newSociety.SocietyId;
                obj.SocietyName = newSociety.SocietyName;
            }
        }

        private async Task<IEnumerable<SocietyReferenceDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _societies;

            var filteredSocieties = _societies.Where(x => x.SocietyName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredSocieties;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _societies = new();

            List<SocietyDto> allsocieties = (await Mediator.Send(new GetAllSocietiesQuery { })).Value.OrderBy(x => x.Name).ToList();
            foreach (SocietyDto society in allsocieties)
            {
                _societies.Add(new SocietyReferenceDto { SocietyId = society.Id, SocietyName = society.Name });
            }

            StateHasChanged();
        }

        private async void addSociety()
        {
            SocietyReferenceDto firstSociety = _societies.First();
            if (Value == null) Value = new List<SocietyReferenceDto>();
            Value.Add(new SocietyReferenceDto { SocietyId = firstSociety.SocietyId, SocietyName = firstSociety.SocietyName });

            await ValueChanged.InvokeAsync(Value);

            StateHasChanged();
        }

        private async void removeSociety(SocietyReferenceDto society)
        {
            
            Value.Remove(society);
            ValueChanged.InvokeAsync(Value);

            StateHasChanged();
            
        }
    }
}
