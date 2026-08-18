using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Severity = MudBlazor.Severity;
using FluentValidation;
using MudBlazor;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Rights.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Society.Commands;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components.Societies
{
    public partial class SocietyRightsGrid
    {
        private MudTable<RightDto> table;
        private string _searchString = null;
        private List<SocietyRightsDto> _rights;
        
        private SocietyRightsDto _selectedItem = new();
        private List<RightsTypeDto> _rightsTypes;
        private SocietyRightsDto _model = new();
        private List<CountryDto> _countries;

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public bool Loading { get; set; }= true;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Works { get; set; } = false;

        [Parameter]
        public RightsSource RightsSource { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        [Parameter]
        public int FilterClientID { get; set; }
        
        [Parameter]
        public int? FilterWorkID { get; set; }

        [Parameter]
        public int FilterCatID { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Open)
            {
                await Task.Run(LoadRights);
            }
        }

        private async Task LoadRights()
        {
            var result = await Mediator.Send(new GetSocietyQuery { Id = Id });

            if (result.IsFailure) return;

            _rights = result.Value.SocietyRights.ToList();

            _rightsTypes = await RefDataService.GetRightsType();
            _countries = await RefDataService.GetCountries();
            Loading = false;
        }

        private bool Filter(SocietyRightsDto right) => FilterBySearchString(right, _searchString);

        private static bool FilterBySearchString(SocietyRightsDto right, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(right.RightsType.Name)
                   || right.RightsType.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                   || (!string.IsNullOrWhiteSpace(right.RightsType.Description) && right.RightsType.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IEnumerable<RightsTypeDto>> SearchRightsTypes(string value, CancellationToken token)
        {
            return string.IsNullOrEmpty(value) ? _rightsTypes : _rightsTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<CountryDto>> SearchCountries(string value, CancellationToken token)
        {
            return string.IsNullOrEmpty(value) ? _countries : _countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task AddSocietyRight()
        {
            var result = await Mediator.Send(new AddSocietyRightCommand { SocietyId = Id, SocietyRightsDto = _model });

            if (result.IsSuccess)
                Snackbar.Add($"{_model.RightsType.Name} added for {_model.Country.Name}", Severity.Success);
            else
                Snackbar.Add(result.Error, Severity.Error);

            await LoadRights();
            StateHasChanged();
        }

        private async Task DeleteRight(SocietyRightsDto context)
        {
            var result = await Mediator.Send(new DeleteSocietyRightCommand { SocietyId = Id, SocietyRightsId = context.Id});

            if (result.IsSuccess)
                Snackbar.Add($"{context.RightsType.Name} deleted", Severity.Success);
            else
                Snackbar.Add(result.Error, Severity.Error);

            await LoadRights();
            StateHasChanged();
        }
    }
}