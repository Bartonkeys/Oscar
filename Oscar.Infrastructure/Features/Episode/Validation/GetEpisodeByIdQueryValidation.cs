using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Queries;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class GetEpisodeByIdQueryValidation: AbstractValidator<GetEpisodeByIdQuery>
    {
        public GetEpisodeByIdQueryValidation()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
