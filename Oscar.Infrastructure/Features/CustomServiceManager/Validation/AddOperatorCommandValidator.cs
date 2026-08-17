using FluentValidation;
using Oscar.Infrastructure.Features.CustomServiceManager.Commands;

namespace Oscar.Infrastructure.Features.CustomServiceManager.Validation
{
    public class AddOperatorCommandValidator: AbstractValidator<AddOperatorCommand>
    {
        public AddOperatorCommandValidator()
        {
            RuleFor(r => r.OperatorDto.FullName).NotNull().NotEmpty();
        }
    }
}
