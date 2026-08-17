using FluentValidation;
using Oscar.Infrastructure.Features.Society.Commands;

namespace Oscar.Infrastructure.Features.Society.Validation;

public class AddSocietyCommandValidation : AbstractValidator<AddSocietyCommand>
{
    public AddSocietyCommandValidation()
    {
        RuleFor(s => s.SocietyDto).NotNull();
        RuleFor(s => s.SocietyDto.Id).Equal(0).WithMessage("Society already exists");
        RuleFor(s => s.SocietyDto.Name).NotNull().NotEmpty().WithMessage("Society requires name");
    }
}