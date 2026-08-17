using FluentValidation;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Validation
{
    public class DeleteRegistrationWorksCommandValidation : AbstractValidator<DeleteRegistrationWorksCommand>
    {

        public DeleteRegistrationWorksCommandValidation()
        {
        }
    }
}
