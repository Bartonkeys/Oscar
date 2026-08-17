using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class GetSeriesTitlesByCatalogueIdValidation : AbstractValidator<GetSeriesTitlesByCatalogueIdQuery>
    {
        public GetSeriesTitlesByCatalogueIdValidation()
        {
            RuleFor(r => r.CatalogueId).GreaterThan(0);
        }
    }
}
