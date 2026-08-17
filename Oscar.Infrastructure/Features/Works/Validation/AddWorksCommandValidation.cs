using FluentValidation;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class AddWorksCommandValidation : AbstractValidator<AddWorksCommand>
    {
        public AddWorksCommandValidation(IValidator<WorksDto> worksDtoValidator)
        {
            RuleFor(r => r.WorksDto).NotNull();
            RuleFor(r => r.WorksDto).SetValidator(worksDtoValidator);
        }
    }
}