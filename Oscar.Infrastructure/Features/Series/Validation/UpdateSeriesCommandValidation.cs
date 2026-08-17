using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class UpdateSeriesCommandValidation: AbstractValidator<UpdateSeriesCommand>
    {

        public UpdateSeriesCommandValidation(IValidator<SeriesUpdateDto> seriesUpdateDtoValidator)
        {
            RuleFor(r => r.SeriesUpdateDto).NotNull();
            RuleFor(r => r.Id).NotEqual(0);
            RuleFor(r => r.SeriesUpdateDto).SetValidator(seriesUpdateDtoValidator);
        }
    }
}
