using FluentValidation;
using Oscar.Infrastructure.Features.Season.Queries;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class GetSeasonTitlesBySeriesIdValidation : AbstractValidator<GetSeasonTitlesBySeriesIdQuery>
    {
        public GetSeasonTitlesBySeriesIdValidation()
        {
            RuleFor(r => r.SeriesId).GreaterThan(0);
        }
    }
}
