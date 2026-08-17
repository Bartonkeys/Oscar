using FluentValidation;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Series.Validation;

public class UpdateSeriesStatusAllCommandValidation : AbstractValidator<UpdateSeriesStatusAllCommand>
{

    public UpdateSeriesStatusAllCommandValidation()
    {
        RuleFor(r => r.SeriesId).NotEqual(0);
        RuleFor(r => r.WorksStatus).NotNull();
    }
}