using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Episode.Commands;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class UpdateEpisodeCommandValidation: AbstractValidator<UpdateEpisodeCommand>
    {

        public UpdateEpisodeCommandValidation(IValidator<EpisodeUpdateDto> episodeUpdateDtoValidator)
        {
            RuleFor(r => r.Id).NotEqual(0);
            RuleFor(r => r.EpisodeUpdateDto).NotNull();
            RuleFor(r => r.EpisodeUpdateDto).SetValidator(episodeUpdateDtoValidator);
        }
    }
}
