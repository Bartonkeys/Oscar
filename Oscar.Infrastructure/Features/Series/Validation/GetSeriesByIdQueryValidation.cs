using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class GetSeriesByIdQueryValidation: AbstractValidator<GetSeriesByIdQuery>
    {
        public GetSeriesByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
