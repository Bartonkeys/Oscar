using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Registration.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class RegistrationsForm
    {
        private bool _processing = false;
        private DateTime? _dateRegistered;
        MudForm form;
        readonly RegistrationsValidator registrationsValidator = new();

        [Parameter]
        public int WorksId { get; set; }

        [Parameter]
        public int? ClientId { get; set; }

        [Parameter]
        public RegistrationDisplayDto? RegistrationDisplayDto { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public IEnumerable<SocietyDto>? Societies { get; set; }

        [Parameter]
        public EventCallback<bool> toggleDrawer { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSocieties();

            if (RegistrationDisplayDto != null)
            {
                _dateRegistered = RegistrationDisplayDto.DateRegistered ?? DateTime.Now;
            }
        }

        private void DateRegisteredChanged(DateTime? date)
        {
            _dateRegistered = date;
        }

        private async Task LoadSocieties()
        {
            if (Societies != null) return;
            Societies = await RefDataService.GetSocieties();
        }

        private async Task Submit()
        {
            _processing = true;
            await form.Validate();

            if (form.IsValid)
            {
                if (RegistrationDisplayDto?.Id > 0)
                {
                    var editRegistrationCommand = new EditRegistrationCommand()
                    {
                        Id = RegistrationDisplayDto.Id,
                        SocietyId = RegistrationDisplayDto.SocietyId,
                        DateRegistered = _dateRegistered
                    };

                    var result = await Mediator.Send(editRegistrationCommand);
                    await HandleResult<RegistrationDisplayDto>(result);
                }
                else
                {
                    var addRegistrationCommand = new AddRegistrationCommand()
                    {
                        WorksId = WorksId,
                        SocietyId = RegistrationDisplayDto.SocietyId,
                        ClientId = ClientId,
                        DateRegistered = _dateRegistered
                    };

                    var result = await Mediator.Send(addRegistrationCommand);
                    await HandleResult<RegistrationDisplayDto>(result);
                }
                await toggleDrawer.InvokeAsync(false);
            }
            else
            {
                await toggleDrawer.InvokeAsync(true);
            }
            _processing = false;
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Right successfully saved", Severity.Success);
                await RefreshParent.InvokeAsync(true);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        protected async Task Cancel()
        {
            await toggleDrawer.InvokeAsync(false);
        }

        public class RegistrationsValidator : AbstractValidator<RegistrationDisplayDto>
        {
            public RegistrationsValidator()
            {
                RuleFor(x => x.SocietyId)
                   .NotNull()
                   .NotEmpty()
                   .GreaterThan(0);
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<RegistrationDisplayDto>.CreateWithOptions((RegistrationDisplayDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
