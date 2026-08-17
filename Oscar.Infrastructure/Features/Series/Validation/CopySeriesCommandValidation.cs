using FluentValidation;
using Oscar.Infrastructure.Features.Series.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class CopySeriesCommandValidation : AbstractValidator<CopySeriesCommand>
    {

        public CopySeriesCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
