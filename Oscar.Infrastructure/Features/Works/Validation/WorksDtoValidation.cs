using FluentValidation;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class WorksDtoValidation : AbstractValidator<WorksDto>
    {
        public WorksDtoValidation()
        {

            RuleFor(r => r.DurationMinutes)
                .NotNull().WithMessage("Please Enter Value for DurationMinutes")
                .GreaterThan(0).WithMessage("DurationMinutes must be greater than 0.");
        }
    }
}