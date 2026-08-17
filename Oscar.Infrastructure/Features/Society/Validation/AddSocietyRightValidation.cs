using FluentValidation;
using Oscar.Infrastructure.Features.Society.Commands;

namespace Oscar.Infrastructure.Features.Society.Validation;

public class AddSocietyRightValidation : AbstractValidator<AddSocietyRightCommand>
{
    public AddSocietyRightValidation()
    {
        RuleFor(s => s.SocietyId).NotEqual(0);
        RuleFor(s => s.SocietyRightsDto).NotNull();
    }
}