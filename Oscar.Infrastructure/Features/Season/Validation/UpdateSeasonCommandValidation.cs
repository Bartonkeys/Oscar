using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Season.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class UpdateSeasonCommandValidation: AbstractValidator<UpdateSeasonCommand>
    {
        public UpdateSeasonCommandValidation(IValidator<SeasonUpdateDto> seasonUpdateDtoValidator)
        {
            RuleFor(r => r.SeasonUpdateDto).NotNull();
            RuleFor(r => r.Id).NotEqual(0);
            RuleFor(r => r.SeasonUpdateDto).SetValidator(seasonUpdateDtoValidator);
        }
    }
}
