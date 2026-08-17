using FluentValidation;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class GetMatchResultByIdQueryValidation: AbstractValidator<GetMatchResultByIdQuery>
    {
        public GetMatchResultByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
