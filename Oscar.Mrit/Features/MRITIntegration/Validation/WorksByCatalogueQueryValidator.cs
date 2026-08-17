using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class WorksByCatalogueQueryValidator: AbstractValidator<WorksByCataloguesQuery>
    {
        public WorksByCatalogueQueryValidator()
        {
            RuleFor(x => x.Catalogues).NotEmpty().WithMessage("No catalogues");
        }
    }
}
