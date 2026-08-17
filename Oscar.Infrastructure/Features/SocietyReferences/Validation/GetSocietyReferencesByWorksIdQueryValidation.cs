using FluentValidation;
using Oscar.Infrastructure.Features.SocietyReferences.Queries;

namespace Oscar.Infrastructure.Features.SocietyReferences.Validation
{
    public class GetSocietyReferencesByWorksIdQueryValidation : AbstractValidator<GetSocietyReferencesByWorksIdQuery>
    {
        public GetSocietyReferencesByWorksIdQueryValidation()
        {
            RuleFor(r => r.WorksId).GreaterThan(0);
        }
    }
}
