using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Matching.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Match
{
    public partial class CreateMatchForm
    {
        MudForm form;

        readonly WorksImportRequestValidator orderValidator = new();

        MatchRequestViewModel model = new();

        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public List<CountryDto> Countries { get; set; } = new();

        [Parameter]
        public List<EnumDTO> Rules { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public string? Title { get; set; } = string.Empty;

        [Parameter]
        public List<RightsTypeDto> RightsTypes { get; set; } = new();

        private bool _processing;
        private bool _default;

        protected override async Task OnParametersSetAsync()
        {
            var rule = Rules.FirstOrDefault(r => r.Name == "None");
            if (rule != null) rule.Selected = true;
        }

        private IEnumerable<int?> Years
        {
            get
            {
                for (var i = DateTime.Now.Year - 50; i <= DateTime.Now.Year + 5; i++)
                {
                    yield return i;
                }
            }
        }

        private async Task Submit()
        {
            if (model.MatchFile == null)
            {
                Snackbar.Add("Please upload a file", Severity.Error);
                return;
            }

            _processing = true;

            PopulateRules();

            await form.Validate();

            if (form.IsValid)
            {
                Stream memoryStream = new MemoryStream();
                await model.MatchFile.OpenReadStream(maxAllowedSize:  10000000).CopyToAsync(memoryStream);

                var formFile = new FormFile(memoryStream, 0, model.MatchFile.Size, model.MatchFile.Name, model.MatchFile.Name);

                var matchRequest = new MatchRequestAddDto()
                {
                    ClientId = model.Client?.Id,
                    FormFile = formFile,
                    RightsFromYear = model.RightsFromYear,
                    RightsToYear = model.RightsToYear,
                    Rules = BuildRules(),
                    TerritoryId = model.Country?.Id,
                    RequestedBy = model.RequestedBy,
                    IgnoreCharactersFollowing = model.IgnoreCharactersAfter,
                    RightsType = model.RightsType,
                };
                var result = await Mediator.Send(new AddMatchRequestCommand
                {
                    MatchRequestAddDto = matchRequest
                });

                await HandleResult(result);

            }

            _processing = false;
        }

        private MatchRules? BuildRules() => model.Rules.Aggregate(new MatchRules(), (current, enumDto) =>
        {
            return current | Enum.Parse<MatchRules>(enumDto.Name);
        });


        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                await OnSubmit.InvokeAsync();
                Snackbar.Add("Submitted!", Severity.Success);
                model = new();
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task<IEnumerable<ClientBasicDto>> SearchClients(string value)
        {
            return string.IsNullOrEmpty(value) ? Clients : Clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<int?>> SearchYears(string value)
        {
            return string.IsNullOrEmpty(value) ? Years : Years.Where(x => x.ToString().Contains(value));
        }

        private async Task<IEnumerable<CountryDto>> SearchCountries(string value)
        {
            return string.IsNullOrEmpty(value) ? Countries : Countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<RightsTypeDto>> SearchRightsTypes(string value)
        {
            return string.IsNullOrEmpty(value) ? RightsTypes : RightsTypes.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public class WorksImportRequestValidator : AbstractValidator<MatchRequestViewModel>
        {
            public WorksImportRequestValidator()
            {
                RuleFor(x => x.RequestedBy)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Rules)
                    .NotEmpty();

                RuleFor(x => x.Rules)
                    .Must(r => r.Any())
                    .NotEmpty();

                RuleFor(x => x.MatchFile)
                    .NotNull();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<MatchRequestViewModel>.CreateWithOptions((MatchRequestViewModel)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        public class MatchRequestViewModel
        {
            public string RequestedBy { get; set; }
            public ClientBasicDto Client { get; set; }
            public IBrowserFile? MatchFile { get; set; }
            public IList<EnumDTO> Rules { get; set; } = new List<EnumDTO>();
            public EnumDTO Rule { get; set; }
            public int? ProductionYear { get; set; }
            public int? RightsFromYear { get; set; }
            public int? RightsToYear { get; set; }
            public CountryDto Country { get; set; }
            public string IgnoreCharactersAfter { get; set; }
            public RightsTypeDto RightsType { get; set; }
        }

        private void PopulateRules()
        {
            foreach (var rule in Rules)
            {
                if (rule.Selected && model.Rules.All(r => r.Name != rule.Name))
                {
                    model.Rules.Add(rule);
                }
            }
        }
    }
}
