using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.StandAlone.Commands;

namespace Oscar.Infrastructure.Features.StandAlone.Validation
{
    public class AddStandAloneCommandValidation: AbstractValidator<AddStandAloneCommand>
    {
        public AddStandAloneCommandValidation(IValidator<StandAloneAddDto> standAloneAddDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.StandAloneAddDto).NotNull();
            RuleFor(r => r.StandAloneAddDto).SetValidator(standAloneAddDtoValidator);
        }
    }
}
