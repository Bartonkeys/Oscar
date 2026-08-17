using FluentValidation;
using Oscar.Infrastructure.Features.Society.Commands;

namespace Oscar.Infrastructure.Features.Society.Validation;

public class DeleteSocietyRightValidation : AbstractValidator<DeleteSocietyRightCommand>
{
    public DeleteSocietyRightValidation()
    {
        RuleFor(s => s.SocietyId).NotEqual(0);
        RuleFor(s => s.SocietyRightsId).NotEqual(0);
    }
}