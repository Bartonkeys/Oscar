using FluentValidation;
using Oscar.Infrastructure.Features.Season.Queries;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class GetSeasonByIdQueryValidation: AbstractValidator<GetSeasonByIdQuery>
    {
        public GetSeasonByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
