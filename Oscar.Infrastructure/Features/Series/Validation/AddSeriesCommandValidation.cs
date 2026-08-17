using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Series.Validation
{
    public class AddSeriesCommandValidation: AbstractValidator<AddSeriesCommand>
    {

        public AddSeriesCommandValidation(IValidator<SeriesAddDto> seriesAddDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.SeriesAddDto).NotNull();
            RuleFor(r => r.SeriesAddDto).SetValidator(seriesAddDtoValidator);

        }
    }
}
