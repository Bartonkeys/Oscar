using FluentValidation;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Clients.Validation
{
    public class AddClientCommandValidation : AbstractValidator<AddClientCommand>
    {
        private OscarContext _context;

        public AddClientCommandValidation(IValidator<ClientAddDto> clientDtoValidator, OscarContext context)
        {
            _context = context;

            RuleFor(r => r.ClientAddDto).NotNull();
            RuleFor(r => r.ClientAddDto).SetValidator(clientDtoValidator);

            RuleFor(r => r.ClientAddDto.ClientReference).Must(ClientReferenceNotAlreadyInUse).When(r => r.ClientAddDto.ClientReference != null).WithMessage("Another record uses this client reference");
        }

        private bool ClientReferenceNotAlreadyInUse(int? clientReference)
        {
            return _context.Clients.Count(c => c.ClientReference == clientReference) == 0;
        }

    }
}