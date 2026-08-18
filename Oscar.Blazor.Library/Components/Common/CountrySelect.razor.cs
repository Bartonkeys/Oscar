using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class CountrySelect
    {
        private CountryDto? _country;

        [Parameter]
        public String Style { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public ICollection<CountryDto> Value { get; set; }

        [Parameter]
        public List<CountryDto>? _countries { get; set; }

        [Parameter]
        public EventCallback<ICollection<CountryDto>> ValueChanged { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _country = default;
           
            if (Value.Any())
            {
                _countries?.RemoveAll(x => Value.Select(c => c.Id).ToList().Contains(x.Id));
            }
            _country = _countries?.FirstOrDefault(x => x.Name == "WORLD");
            StateHasChanged();
        }

        private async Task<IEnumerable<CountryDto>> Search(string value, CancellationToken token)
        {
            if(_countries == null || !_countries.Any())
                _countries = (await Mediator.Send(new GetAllCountriesQuery())).Value.OrderBy(x => x.Name).ToList();

            if (string.IsNullOrEmpty(value))
                return _countries;

            var filteredCountries = _countries.Where(x =>
            x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredCountries;
        }

        private void AddCountry()
        {
            if (_country != null && !Value.Contains(_country))
            {
                Value.Add(_country);
                if (_countries != null)
                {
                    _countries.Remove(_country);
                }

                _country = default;
            }
        }

        private void RemoveCountry(CountryDto country)
        {
            Value.Remove(country);
            if (_countries != null)
            {
                _countries.Add(country);
                _countries = _countries.OrderBy(x => x.Name).ToList();
            }

            _country = default;
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

