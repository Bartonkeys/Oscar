using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class UnsuccessfulMatchStatusQueryValidator : AbstractValidator<UnsuccessfulMatchStatusQuery>
    {
        public UnsuccessfulMatchStatusQueryValidator()
        {
        }
    }
}