using FluentValidation;
using Oscar.Infrastructure.Features.Matching.Queries;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class GetMatchRequestByIdQueryValidation: AbstractValidator<GetMatchRequestByIdQuery>
    {
        public GetMatchRequestByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
