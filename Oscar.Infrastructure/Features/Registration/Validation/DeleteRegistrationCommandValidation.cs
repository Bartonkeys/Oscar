using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Validation
{
    public class DeleteRegistrationCommandValidation : AbstractValidator<DeleteRegistrationCommand>
    {

        public DeleteRegistrationCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
