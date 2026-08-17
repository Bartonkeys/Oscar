using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Commands;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class UpdateMatchStatusValidator: AbstractValidator<UpdateMatchStatusCommand>
    {
        public UpdateMatchStatusValidator()
        {
            RuleForEach(m => m.Statuses).NotNull().WithMessage("Status collection is null");
            RuleForEach(m => m.Statuses).Must(status => status.WorksId > 0)
                .WithMessage("Works Id must be greater then zero");
        }
    }
}
