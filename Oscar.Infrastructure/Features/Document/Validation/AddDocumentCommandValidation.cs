using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Document.Commands;

namespace Oscar.Infrastructure.Features.Document.Validation
{
    public class AddDocumentCommandValidation : AbstractValidator<AddDocumentCommand>
    {
        public AddDocumentCommandValidation()
        {
            RuleFor(r => r.DocumentDto).NotNull().WithMessage("Document is required");
            RuleFor(r => r.DocumentDto.FormFile).NotNull().WithMessage("File is required");
        }
    }
}
