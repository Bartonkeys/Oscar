using FluentValidation;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class DeleteWorksImportCommandValidator : AbstractValidator<DeleteWorksImportCommand>
    {
        public DeleteWorksImportCommandValidator()
        {
        }
    }    
}
