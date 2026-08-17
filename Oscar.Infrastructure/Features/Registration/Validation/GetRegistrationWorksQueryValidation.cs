using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Infrastructure.Features.Registration.Validation;

public class GetRegistrationWorksQueryValidation : AbstractValidator<GetRegistrationWorksQuery>
{
    public GetRegistrationWorksQueryValidation()
    {
        RuleFor(q => q.ClientId).NotNull().NotEqual(0);
        RuleFor(q => q.SocietyId).NotNull().NotEqual(0);
    }
}