using FluentValidation;
using Oscar.Infrastructure.Features.Conflict.Queries;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class GetConflictsByWorksIdQueryValidation : AbstractValidator<GetConflictsByWorksIdQuery>
    {
        public GetConflictsByWorksIdQueryValidation()
        {
            RuleFor(r => r.WorksId).GreaterThan(0);
        }
    }
}
