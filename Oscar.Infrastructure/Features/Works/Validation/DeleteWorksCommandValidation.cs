using FluentValidation;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Validation;

public class DeleteWorksCommandValidation : AbstractValidator<DeleteWorksCommand>
{
    public DeleteWorksCommandValidation(IValidator<WorksDto> worksDtoValidator)
    {
        RuleFor(r => r.WorksDto)
            .NotNull()
            .WithMessage("You must supply a valid Works object to delete");

        RuleFor(r => r.WorksDto.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Value must be a valid Works Id to delete");

        //TODO: Validation for fields which cannot be deleted due to referential integrity
    }
}