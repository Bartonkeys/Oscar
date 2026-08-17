using FluentValidation;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Infrastructure.Features.Works.Validation
{
    public class GetWorksByIdQueryValidation: AbstractValidator<GetWorksByIdQuery>
    {
        public GetWorksByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
