using FluentValidation;
using Oscar.Infrastructure.Features.Episode.Commands;

namespace Oscar.Infrastructure.Features.Episode.Validation
{
    public class BulkAddEpisodeCommandValidation : AbstractValidator<BulkAddEpisodeCommand>
    {
        public BulkAddEpisodeCommandValidation()
        {
        }
    }
}
