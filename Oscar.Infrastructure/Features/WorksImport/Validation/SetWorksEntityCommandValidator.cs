using FluentValidation;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Infrastructure.Features.WorksImport.Validation;

public class SetWorksEntityCommandValidator : AbstractValidator<SetWorksEntityCommand>
{
    public SetWorksEntityCommandValidator()
    {

    }
}