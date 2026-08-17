using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.SocietyReferences.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class SocietyReferenceForm
    {
        private bool _processing = false;
        MudForm form;
        readonly SocietyReferenceValidator societyReferenceValidator = new();

        [Parameter]
        public int WorksId { get; set; }

        [Parameter]
        public int? ClientId { get; set; }

        [Parameter]
        public SocietyReferenceDto? SocietyReferenceDto { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public EventCallback<bool> toggleDrawer { get; set; }

        [Parameter]
        public IEnumerable<SocietyDto>? Societies { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSocieties();
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
                if (SocietyReferenceDto?.Id > 0)
                {
                    var editSocietyReferenceCommand = new EditSocietyReferenceCommand()
                    {
                        Id = SocietyReferenceDto.Id,
                        SocietyId = SocietyReferenceDto.SocietyId,
                        Reference = SocietyReferenceDto.Reference

                    };

                    var result = await Mediator.Send(editSocietyReferenceCommand);
                    await HandleResult<SocietyReferenceDto>(result);
                }
                else
                {
                    var addSocietyReferenceCommand = new AddSocietyReferenceCommand()
                    {
                        WorksId = WorksId,
                        SocietyId = SocietyReferenceDto.SocietyId,
                        Reference = SocietyReferenceDto.Reference,
                    };

                    var result = await Mediator.Send(addSocietyReferenceCommand);
                    await HandleResult<SocietyReferenceDto>(result);
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
                Snackbar.Add("SocietyReference successfully saved", Severity.Success);
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

        public class SocietyReferenceValidator : AbstractValidator<SocietyReferenceDto>
        {
            public SocietyReferenceValidator()
            {
                RuleFor(x => x.Reference)
                   .NotNull()
                   .NotEmpty();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<SocietyReferenceDto>.CreateWithOptions((SocietyReferenceDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

    }
}
