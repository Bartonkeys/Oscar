using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class WorksByIdQueryValidator : AbstractValidator<WorksByIdQuery>
    {
        public WorksByIdQueryValidator()
        {
            RuleFor(x => x.WorksIds).NotEmpty().WithMessage("Must have at least 1 worksId");
        }
    }
}
