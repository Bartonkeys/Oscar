using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.ProductionCompany.Commands;
using Oscar.Infrastructure.Features.ProductionCompany.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Companies
    {
        private List<CompanyDto> _companies;
        private CompanyDto _company;
        private String _email;
        private String _name;

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public ICollection<CompanyDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<CompanyDto>> ValueChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadCompanies();
        }

        private async Task LoadCompanies()
        {
            _companies = (await Mediator.Send(new GetAllCompaniesQuery())).Value.OrderBy(x => x.Name).ToList();
            if (Value is { Count: > 0 })
            {
                _companies.RemoveAll(x => Value.Select(c => c.Id).ToList().Contains(x.Id));
            }
            StateHasChanged();

        }
        private async Task<IEnumerable<CompanyDto>> Search(string value)
        {
            if (_companies == null)
            {
                await LoadCompanies();
            }

            if (string.IsNullOrEmpty(value))
            {
                return _companies;
            }

            var filteredCompanies = _companies.Where(x =>
            x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredCompanies;
        }

        private async void AddCompany()
        {
            if (_company != null && !Value.Contains(_company))
            {
                Value.Add(_company);
            }
            _companies.Remove(_company);
            await Task.CompletedTask;
        }

        private async void RemoveCompany(CompanyDto company)
        {
            Value.Remove(company);
            _companies.Add(company);
            await Task.CompletedTask;
        }

        private async void CreateCompany()
        {
            var addCompanyCommand = new AddCompanyCommand()
            {
                CompanyAddDto = new CompanyAddDto
                {
                    Email = _email,
                    Name = _name
                }
            };
            var resultCompany = await Mediator.Send(addCompanyCommand);
            await HandleResult<CompanyDto>(resultCompany);
        }

        private async Task HandleResult<T>(Result<CompanyDto> result) where T : CompanyDto
        {
            if (result.IsSuccess)
            {
                Value.Add(result.Value);
                StateHasChanged();
                Snackbar.Add("Successfully created company", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
            await Task.CompletedTask;
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

