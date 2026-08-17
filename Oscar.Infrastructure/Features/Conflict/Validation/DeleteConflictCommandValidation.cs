using FluentValidation;
using Oscar.Infrastructure.Features.Conflict.Commands;

namespace Oscar.Infrastructure.Features.Conflict.Validation
{
    public class DeleteConflictCommandValidation : AbstractValidator<DeleteConflictCommand>
    {

        public DeleteConflictCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
