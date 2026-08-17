using FluentValidation;
using Oscar.Infrastructure.Features.Catalogue.Commands;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;

namespace Oscar.Infrastructure.Features.Catalogue.Validation;

public class UpdateCatalogueCommandValidation : AbstractValidator<UpdateCatalogueCommand>
{
    public UpdateCatalogueCommandValidation(IValidator<CatalogueAddDto> catalogueDtoValidator, OscarContext context)
    {

        RuleFor(r => r.CatalogueUpdateDto)
            .NotNull();

        RuleFor(r => r.Id)
            .NotEqual(0)
            .WithMessage("Id must not be equal to 0");
    }
}