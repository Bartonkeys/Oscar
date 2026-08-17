using FluentValidation;
using Oscar.Infrastructure.Features.Conflict.Commands;

namespace Oscar.Infrastructure.Features.Conflict.Validation
{
    public class AddConflictCommandValidator : AbstractValidator<AddConflictCommand>
    {
        public AddConflictCommandValidator()
        {
        }

    }


}
