using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation;

public class DeleteRightsFromHeaderCommandValidation : AbstractValidator<DeleteRightsFromHeaderCommand>
{
    public DeleteRightsFromHeaderCommandValidation()
    {
        RuleFor(r => r.Id).NotEqual(0);
    }
}