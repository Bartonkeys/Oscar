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
using Oscar.Infrastructure.Features.Screenrights.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Screenrights
{
    public partial class CreateScreenrightsForm
    {
        MudForm form;

        readonly ScreenrightsRequestValidator orderValidator = new();

        ScreenrightsViewModel model = new();

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
            if (model.ScreenrightsFile == null)
            {
                Snackbar.Add("Please upload a file", Severity.Error);
                return;
            }

            _processing = true;

            await form.Validate();

            if (form.IsValid)
            {
                Stream memoryStream = new MemoryStream();
                await model.ScreenrightsFile.OpenReadStream(maxAllowedSize:  10000000).CopyToAsync(memoryStream);

                var formFile = new FormFile(memoryStream, 0, model.ScreenrightsFile.Size, model.ScreenrightsFile.Name, model.ScreenrightsFile.Name);

                var screenrightsRequest = new ScreenrightsRequestDto()
                {
                    RequestedBy = model.RequestedBy,
                    FormFile = formFile
                };
                var result = await Mediator.Send(new AddScreenrightsRequestCommand
                {
                    ScreenrightsRequestDto = screenrightsRequest
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


        public class ScreenrightsRequestValidator : AbstractValidator<ScreenrightsViewModel>
        {
            public ScreenrightsRequestValidator()
            {
                RuleFor(x => x.RequestedBy)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.ScreenrightsFile)
                    .NotNull();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<ScreenrightsViewModel>.CreateWithOptions((ScreenrightsViewModel)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        public class ScreenrightsViewModel
        {
            public string RequestedBy { get; set; }
            public IBrowserFile? ScreenrightsFile { get; set; }
        }
    }
}
