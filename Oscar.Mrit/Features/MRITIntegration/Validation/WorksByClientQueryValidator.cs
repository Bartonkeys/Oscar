using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class WorksByClientQueryValidator: AbstractValidator<WorksByClientQuery>
    {
        public WorksByClientQueryValidator()
        {
            RuleFor(x => x.ClientId).NotEqual(0).WithMessage("Client Id is not set");
        }
    }
}
