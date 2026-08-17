using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Season.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class AddSeasonCommandValidation: AbstractValidator<AddSeasonCommand>
    {
        public AddSeasonCommandValidation(IValidator<SeasonAddDto> seasonAddDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.SeasonAddDto).NotNull();
            RuleFor(r => r.SeasonAddDto).SetValidator(seasonAddDtoValidator);
        }

    }
}
