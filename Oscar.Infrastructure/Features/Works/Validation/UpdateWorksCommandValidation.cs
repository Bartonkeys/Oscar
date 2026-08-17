using FluentValidation;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class UpdateWorksCommandValidation : AbstractValidator<UpdateWorksCommand>
{
    public UpdateWorksCommandValidation(IValidator<WorksDto> worksDtoValidator)
    {
        RuleFor(r => r.WorksDto)
            .NotNull()
            .WithMessage("You must supply a valid Works object to update");

        RuleFor(r => r.WorksDto.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Value must be a valid Works Id to update");

        RuleFor(r => r.WorksDto).SetValidator(worksDtoValidator);
    }
}