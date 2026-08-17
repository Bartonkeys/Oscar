using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Commands;
using Oscar.Core.DTOs;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Catalogue.Validation
{
    public class AddCatalogueCommandValidation : AbstractValidator<AddCatalogueCommand>
    {
        private OscarContext _context;

        public AddCatalogueCommandValidation(IValidator<CatalogueAddDto> catalogueDtoValidator, OscarContext context)
        {
            _context = context;

            RuleFor(r => r.catalogueAddDto).NotNull();
            RuleFor(r => r.catalogueAddDto).SetValidator(catalogueDtoValidator);

        }

    }

    public class CatalogueAddDtoValidation : AbstractValidator<CatalogueAddDto>
    {
        public CatalogueAddDtoValidation()
        {
            RuleFor(c => c.Name).NotNull().NotEmpty();
        }
    }
}