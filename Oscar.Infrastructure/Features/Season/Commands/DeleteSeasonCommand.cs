using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Season.Commands
{
    public class DeleteSeasonCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteSeasonCommandHandler : AbstractBaseHandler<DeleteSeasonCommand, string>
    {
        public DeleteSeasonCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteSeasonCommand> validator, ILogger<DeleteSeasonCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
                .Include(e => e.Companies)
                .Include(e => e.Titles)
                .Include(e => e.Conflicts)
                .Include(e => e.WorksType)
                .Include(e => e.AlternativeTitles)
                .Include(e => e.WorksStatusHistory)
                .Include(e => e.Rights)
                .Include(e => e.Actors)
                .Include(e => e.Producers)
                .Include(e => e.Catalogues)
                .Include(e => e.Countries)
                .Include(e => e.Directors)
                .Include(e => e.Languages)
                .Include(e => e.Clients)
                .Include(e => e.Episodes)
                .Include("Episodes.Companies")
                .Include("Episodes.Titles")
                .Include("Episodes.Conflicts")
                .Include("Episodes.AlternativeTitles")
                .Include("Episodes.WorksStatusHistory")
                .Include("Episodes.Rights")
                .Include("Episodes.Actors")
                .Include("Episodes.Producers")
                .Include("Episodes.Catalogues")
                .Include("Episodes.Countries")
                .Include("Episodes.Directors")
                .Include("Episodes.Languages")
                .Include("Episodes.Clients")
                .AsSplitQuery()
                .FirstOrDefault(s => s.Id == request.Id);

            if (season == null)
            {
                Logger.LogInformation((int)SeasonFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            WorksHelper.RemoveSeason(season, OscarContext);

            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)SeasonFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
