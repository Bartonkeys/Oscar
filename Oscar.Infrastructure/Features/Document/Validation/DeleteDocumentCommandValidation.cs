using FluentValidation;
using Oscar.Infrastructure.Features.Document.Commands;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Document.Validation;

public class DeleteDocumentCommandValidation : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidation()
    {
        RuleFor(r => r.Id)
            .NotEqual(0)
            .WithMessage("Id must not be equal to 0");
    }
}