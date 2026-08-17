using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    public class GetClientByIdQueryValidation: AbstractValidator<GetClientByIdQuery>
    {
        public GetClientByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
