using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    public class GetClientByWorkIdQueryValidation: AbstractValidator<GetClientByWorkIdQuery>
    {
        public GetClientByWorkIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
