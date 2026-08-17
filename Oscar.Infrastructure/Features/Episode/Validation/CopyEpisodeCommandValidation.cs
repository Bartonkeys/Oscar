using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Commands;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class CopyEpisodeCommandValidation : AbstractValidator<CopyEpisodeCommand>
    {

        public CopyEpisodeCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
