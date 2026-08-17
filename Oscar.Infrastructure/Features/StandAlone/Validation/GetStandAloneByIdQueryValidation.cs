using FluentValidation;
using Oscar.Infrastructure.Features.StandAlone.Queries;

namespace Oscar.Infrastructure.Features.StandAlone.Validation
{
    public class GetStandAloneByIdQueryValidation: AbstractValidator<GetStandAloneByIdQuery>
    {
        public GetStandAloneByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
