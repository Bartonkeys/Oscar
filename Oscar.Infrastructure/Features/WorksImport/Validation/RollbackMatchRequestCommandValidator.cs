using FluentValidation;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class RollbackWorksImportRequestCommandValidator : AbstractValidator<RollbackWorksImportRequestCommand>
    {
        public RollbackWorksImportRequestCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }

    public class RollbackWorksImportCommandValidator : AbstractValidator<RollbackWorksImportCommand>
    {
        public RollbackWorksImportCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
}
