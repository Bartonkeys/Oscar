using FluentValidation;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.MRITIntegration.Validation
{
    public class ClientAndCataloguesByWorksValidator : AbstractValidator<ClientAndCatalogueByWorksQuery>
    {
        public ClientAndCataloguesByWorksValidator()
        {
            RuleFor(x => x.WorksIds).NotEmpty().WithMessage("No worksIds");
        }
    }
}
