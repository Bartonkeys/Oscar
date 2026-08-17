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
using Oscar.Infrastructure.Features.Equivalence.Commands;
using Oscar.Infrastructure.Features.Matching.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Equivalence
{
    public partial class CreateEquivalenceForm
    {
        MudForm form;

        readonly EquivalenceRequestValidator orderValidator = new();

        EquivalenceViewModel model = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public string? Title { get; set; } = string.Empty;

        private bool _processing;
        private bool _default;

        protected override async Task OnInitializedAsync()
        {
            var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
            var user = authstate.User;
            if (user != null && user.Identity != null && user.Identity.Name != null)
            {
                model.RequestedBy = user.Identity.Name;
                StateHasChanged();
            }

        }

        private async Task Submit()
        {
            if (model.EquivalenceFile == null)
            {
                Snackbar.Add("Please upload a file", Severity.Error);
                return;
            }

            _processing = true;

            await form.Validate();

            if (form.IsValid)
            {
                Stream memoryStream = new MemoryStream();
                await model.EquivalenceFile.OpenReadStream(maxAllowedSize: 200 * 1024 * 1024).CopyToAsync(memoryStream);

                var formFile = new FormFile(memoryStream, 0, model.EquivalenceFile.Size, model.EquivalenceFile.Name, model.EquivalenceFile.Name);

                var equivalenceRequest = new EquivalenceRequestDto()
                {
                    RequestedBy = model.RequestedBy,
                    FormFile = formFile
                };
                var result = await Mediator.Send(new AddEquivalenceRequestCommand
                {
                    EquivalenceRequestDto = equivalenceRequest
                });

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
            _processing = false;
        }


        public class EquivalenceRequestValidator : AbstractValidator<EquivalenceViewModel>
        {
            public EquivalenceRequestValidator()
            {
                RuleFor(x => x.RequestedBy)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.EquivalenceFile)
                    .NotNull();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<EquivalenceViewModel>.CreateWithOptions((EquivalenceViewModel)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        public class EquivalenceViewModel
        {
            public string RequestedBy { get; set; }
            public IBrowserFile? EquivalenceFile { get; set; }
        }
    }
}
