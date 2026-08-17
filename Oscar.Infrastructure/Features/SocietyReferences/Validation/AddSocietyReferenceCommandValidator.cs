using FluentValidation;
using Oscar.Infrastructure.Features.SocietyReferences.Commands;

namespace Oscar.Infrastructure.Features.SocietyReferences.Validation
{
    public class AddSocietyReferenceCommandValidator : AbstractValidator<AddSocietyReferenceCommand>
    {
        public AddSocietyReferenceCommandValidator()
        {
        }

    }


}
