using FluentValidation;
using Oscar.Infrastructure.Features.Season.Queries;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class GetSeasonBasicByIdQueryValidation: AbstractValidator<GetSeasonBasicByIdQuery>
    {
        public GetSeasonBasicByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
