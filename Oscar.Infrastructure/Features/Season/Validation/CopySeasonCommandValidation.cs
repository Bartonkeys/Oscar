using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Season.Commands;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class CopySeasonCommandValidation : AbstractValidator<CopySeasonCommand>
    {

        public CopySeasonCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
