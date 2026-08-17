using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Clients.Validation;

public class UpdateClientCommandValidation : AbstractValidator<UpdateClientCommand>
{

    private OscarContext _context;

    public UpdateClientCommandValidation(IValidator<ClientAddDto> clientDtoValidator, OscarContext context)
    {
        _context = context;

        RuleFor(r => r.ClientUpdateDto)
            .NotNull();

        RuleFor(r => r.Id)
            .NotEqual(0)
            .WithMessage("Id must not be equal to 0");

        RuleFor(r => r.ClientUpdateDto).SetValidator(clientDtoValidator);
       // RuleFor(r => r.ClientUpdateDto.ClientReference).Must(ClientReferenceNotAlreadyInUse).When(r => r.ClientUpdateDto.ClientReference != null).WithMessage("Another record uses this client reference");

    }

    private bool ClientReferenceNotAlreadyInUse(UpdateClientCommand updateClientCommand, string? clientReference)
    {
        return _context.Clients.Count(c => c.ClientReference == int.Parse(clientReference) && c.Id != updateClientCommand.Id && c.Status == Status.Active_In_Term) == 0;
    }
}