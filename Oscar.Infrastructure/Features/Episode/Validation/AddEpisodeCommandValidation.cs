using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Episode.Commands;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class AddEpisodeCommandValidation: AbstractValidator<AddEpisodeCommand>
    {
        public AddEpisodeCommandValidation(IValidator<EpisodeAddDto> episodeAddDtoValidator, OscarContext context)
        {
            var entityChecker = new EntityChecker(context);

            RuleFor(r => r.EpisodeAddDto).NotNull();
            RuleFor(r => r.EpisodeAddDto).SetValidator(episodeAddDtoValidator);
          
        }
    }
}
