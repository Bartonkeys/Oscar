using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Queries;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class GetEpisodeBySeasonIdQueryValidation: AbstractValidator<GetEpisodeBySeasonIdQuery>
    {
        public GetEpisodeBySeasonIdQueryValidation()
        {
            RuleFor(r => r.SeasonId).GreaterThan(0);
        }
    }
}
