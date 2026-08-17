using FluentValidation;
using Oscar.Infrastructure.Features.SocietyReferences.Commands;

namespace Oscar.Infrastructure.Features.SocietyReferences.Validation
{
    public class EditSocietyReferenceCommandValidator : AbstractValidator<EditSocietyReferenceCommand>
    {
        public EditSocietyReferenceCommandValidator()
        {
        }

    }


}
