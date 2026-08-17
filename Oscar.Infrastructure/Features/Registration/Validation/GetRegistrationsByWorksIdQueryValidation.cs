using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class GetRegistrationsByWorksIdQueryValidation : AbstractValidator<GetRegistrationsByWorksIdQuery>
    {
        public GetRegistrationsByWorksIdQueryValidation()
        {
            RuleFor(r => r.WorksId).GreaterThan(0);
        }
    }
}
