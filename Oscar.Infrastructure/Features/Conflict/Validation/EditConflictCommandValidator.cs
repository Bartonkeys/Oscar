using FluentValidation;
using Oscar.Infrastructure.Features.Conflict.Commands;

namespace Oscar.Infrastructure.Features.Conflict.Validation
{
    public class EditConflictCommandValidator : AbstractValidator<EditConflictCommand>
    {
        public EditConflictCommandValidator()
        {
        }

    }


}
