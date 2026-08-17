using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.StandAlone.Commands;

namespace Oscar.Infrastructure.Features.StandAlone.Validation
{
    public class UpdateStandAloneCommandValidation: AbstractValidator<UpdateStandAloneCommand>
    {

        public UpdateStandAloneCommandValidation(IValidator<StandAloneUpdateDto> standAloneUpdateDtoValidator)
        {
            RuleFor(r => r.StandAloneUpdateDto).NotNull();
            RuleFor(r => r.Id).NotEqual(0);
            RuleFor(r => r.StandAloneUpdateDto).SetValidator(standAloneUpdateDtoValidator);
        }
    }
}
