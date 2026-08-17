using FluentValidation;
using Oscar.Infrastructure.Features.Rights.Commands;

namespace Oscar.Infrastructure.Features.Rights.Validation;

public class UpdateBulkRightsCommandValidation : AbstractValidator<UpdateBulkRightsCommand>
{
    public UpdateBulkRightsCommandValidation()
    {
        RuleFor(r => r.ClientId).NotNull().NotEqual(0);
        RuleFor(r => r.CatalogueId).NotNull().NotEqual(0);
        RuleFor(r => r.Rights).NotNull().NotEmpty();
    }
}