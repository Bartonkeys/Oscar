using FluentValidation;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation;

public class UpdateRightsFromHeaderCommandValidation : AbstractValidator<UpdateRightsFromHeaderCommand>
{
    public UpdateRightsFromHeaderCommandValidation()
    {
        RuleFor(r => r.Id).NotEqual(0);
        RuleFor(r => r.Rights).NotNull().NotEmpty();
        RuleFor(r => r.Discriminator).NotEqual(Discriminator.Episode).NotEqual(Discriminator.StandAlone)
            .WithMessage("Incorrect Works Discriminator");
    }
}