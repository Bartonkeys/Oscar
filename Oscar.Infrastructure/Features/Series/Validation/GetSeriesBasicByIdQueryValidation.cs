using FluentValidation;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class GetSeriesBasicByIdQueryValidation: AbstractValidator<GetSeriesBasicByIdQuery>
    {
        public GetSeriesBasicByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
