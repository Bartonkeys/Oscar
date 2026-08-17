using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Commands;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class DeleteEpisodeCommandValidation: AbstractValidator<DeleteEpisodeCommand>
    {

        public DeleteEpisodeCommandValidation()
        {
            RuleFor(r => r.Id).NotEqual(0);
        }
    }
}
