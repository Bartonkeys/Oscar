using FluentValidation;
using Oscar.Infrastructure.Features.Actor.Queries;

namespace Oscar.Infrastructure.Features.Actor.Validation
{
    public class GetActorQueryValidation : AbstractValidator<GetActorQuery>
    {
        public GetActorQueryValidation()
        {
        }
    }
}
