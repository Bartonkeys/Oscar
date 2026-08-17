using FluentValidation;
using Oscar.Infrastructure.Features.Society.Commands;

namespace Oscar.Infrastructure.Features.Society.Validation;

public class UpdateSocietyCommandValidation : AbstractValidator<UpdateSocietyCommand>
{
    public UpdateSocietyCommandValidation()
    {
        RuleFor(s => s.SocietyDto).NotNull();
    }
}