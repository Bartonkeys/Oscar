using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class Societies
    {
        private List<SocietyDto> _societies;
        private SocietyDto _society;
        private SortDirection _sortDirection = SortDirection.Ascending;
        [Parameter]
        public ICollection<SocietyDto>? Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<SocietyDto>> ValueChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _societies = (await Mediator.Send(new GetAllSocietiesQuery())).Value.OrderBy(x => x.Name).ToList();
            _societies.RemoveAll(x => Value.Select(c => c.Id).ToList().Contains(x.Id));
            SortSocieties();
            StateHasChanged();
        }

        private async Task<IEnumerable<SocietyDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _societies;

            var filteredSocieties = _societies.Where(x =>
            x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return await Task.FromResult(filteredSocieties);
        }

        private void AddSociety()
        {
            if (_society != null && !Value.Contains(_society))
            {
                Value.Add(_society);
            }
            _societies.Remove(_society);
        }

        private void RemoveSociety(SocietyDto society)
        {
            if (Value != null)
            {
                Value.Remove(society);
                _societies.Add(society);
            }
        }

        private void AddAllSocieties()
        {
            var societies = new List<SocietyDto>(_societies);
            foreach (var society in societies)
            {
                _society = society; 
                AddSociety();
            }

            if (societies.Any())
            {
                _society = null;
                SortSocieties();
            }
        }

        private void SortSocieties()
        {
            if (Value != null)
            {
                if (_sortDirection == SortDirection.Descending)
                {
                    Value = Value.OrderByDescending(x => x.Name).ToCollection();
                    _sortDirection = SortDirection.Ascending;
                }
                else
                {
                    Value = Value.OrderBy(x => x.Name).ToCollection();
                    _sortDirection = SortDirection.Descending;
                }
            }
        }
    }
}

