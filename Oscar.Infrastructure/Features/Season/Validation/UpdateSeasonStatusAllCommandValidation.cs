using FluentValidation;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation;

public class UpdateSeasonStatusAllCommandValidation : AbstractValidator<UpdateSeasonStatusAllCommand>
{

    public UpdateSeasonStatusAllCommandValidation()
    {
        RuleFor(r => r.SeasonId).NotEqual(0);
        RuleFor(r => r.WorksStatus).NotNull();
    }
}