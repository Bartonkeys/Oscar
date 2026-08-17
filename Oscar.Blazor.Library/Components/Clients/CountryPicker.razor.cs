using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Country.Queries;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class CountryPicker
    {
        [Parameter]
        public Expression<Func<string?>>? For { get; set; }

        [Parameter]
        public string? Value { get; set; } = "";

        [Parameter]
        public String Label { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<string?> ValueChanged { get; set; }

        [Inject] protected ReferenceDataService RefDataService { get; set; }


        private List<CountryDto> _countries;

        private async Task OnSelectChange(string? newValue)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }

        private async Task<IEnumerable<string>> Search(string value)
        {
            if (_countries == null)
            {
                await LoadCountries();
            }
            if (string.IsNullOrEmpty(value) && _countries != null)
            {
                return _countries.Select(x=> x.Description);
            }

            return _countries.Where(x => x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase)).Select( x => x.Description);
        }

        private async Task LoadCountries()
        {
            _countries = await RefDataService.GetCountries();
        }
    }
    
}
