using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Clients.Validation;

public class DeleteClientCommandValidation : AbstractValidator<DeleteClientCommand>
{
    public DeleteClientCommandValidation()
    {
        RuleFor(r => r.Id)
            .NotEqual(0)
            .WithMessage("Id must not be equal to 0");
    }
}