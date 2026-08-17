using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation;

public class UseCatalogueRightsCommandValidation : AbstractValidator<UseCatalogueRightsCommand>
{
    public UseCatalogueRightsCommandValidation()
    {
        RuleFor(r => r.Id).NotEqual(0);

    }
}