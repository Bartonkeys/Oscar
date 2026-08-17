using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Queries;

namespace Oscar.Infrastructure.Features.Catalogue.Validation
{
    public class GetCatalogueQueryValidation : AbstractValidator<GetCatalogueQuery>
    {
        public GetCatalogueQueryValidation()
        {
        }
    }
}
