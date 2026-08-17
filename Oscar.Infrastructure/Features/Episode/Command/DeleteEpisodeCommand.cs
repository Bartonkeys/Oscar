using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Commands
{
    public class DeleteEpisodeCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteEpisodeCommandHandler : AbstractBaseHandler<DeleteEpisodeCommand, string>
    {
        public DeleteEpisodeCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteEpisodeCommand> validator, ILogger<DeleteEpisodeCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = OscarContext.Episodes
                .Include(e => e.Companies)
                .Include(e => e.Titles)
                .Include(e => e.Conflicts)
                .Include(e => e.WorksType)
                .Include(e => e.AlternativeTitles)
                .Include(e => e.WorksStatusHistory)
                .Include(e => e.Rights)
                .Include(e => e.Actors)
                .Include(e => e.Catalogues)
                .Include(e => e.Countries)
                .Include(e => e.Directors)
                .Include(e => e.Languages)
                .Include(e => e.Clients)
                .Include(e => e.Producers)
                .AsSplitQuery()
                .FirstOrDefault(s => s.Id == request.Id);

            if (episode == null)
            {
                Logger.LogInformation((int)EpisodeFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            WorksHelper.RemoveEpisode(episode, OscarContext);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)EpisodeFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
