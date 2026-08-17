using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Commands;

namespace Oscar.Infrastructure.Features.Catalogue.Validation
{
    public class DeleteCatalogueCommandValidation : AbstractValidator<DeleteCatalogueCommand>
    {

        public DeleteCatalogueCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
