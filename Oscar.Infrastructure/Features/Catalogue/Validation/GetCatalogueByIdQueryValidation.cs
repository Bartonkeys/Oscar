using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Queries;

namespace Oscar.Infrastructure.Features.Catalogues.Validation
{
    public class GetCatalogueByIdQueryValidation: AbstractValidator<GetCatalogueByIdQuery>
    {
        public GetCatalogueByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }

    public class GetCatalogueWithRightsByIdQueryValidation : AbstractValidator<GetCatalogueRightsQuery>
    {
        public GetCatalogueWithRightsByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
