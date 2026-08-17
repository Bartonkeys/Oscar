using FluentValidation;
using Oscar.Infrastructure.Features.SocietyReferences.Commands;

namespace Oscar.Infrastructure.Features.SocietyReferences.Validation
{
    public class DeleteSocietyReferenceCommandValidation : AbstractValidator<DeleteSocietyReferenceCommand>
    {

        public DeleteSocietyReferenceCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
