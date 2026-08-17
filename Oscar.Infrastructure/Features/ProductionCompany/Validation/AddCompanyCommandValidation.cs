using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.ProductionCompany.Commands;

namespace Oscar.Infrastructure.Features.ProductionCompany.Validation
{
    public class AddCompanyCommandValidation: AbstractValidator<AddCompanyCommand>
    {
        public AddCompanyCommandValidation(IValidator<CompanyAddDto>companyAddDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.CompanyAddDto).NotNull();
            RuleFor(r => r.CompanyAddDto).SetValidator(companyAddDtoValidator);
          
        }
    }
}
