using AutoMapper;
using BartonKeys.Functional;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
    public class CopySeasonCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public int NewClientID { get; set; }
        public int NewCatalogueID { get; set; }
        public int? NewSeriesID { get; set; }
        public bool CopyOrMoveUnderlyingWorks { get; set; }
        public bool Relinquish { get; set; }

    }

    public class CopySeasonCommandHandler : AbstractBaseHandler<CopySeasonCommand, string>
    {
        public CopySeasonCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<CopySeasonCommand> validator, ILogger<CopySeasonCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(CopySeasonCommand request, CancellationToken cancellationToken)
        {
            var season = OscarContext.Seasons
                        .Include(w => w.Clients)
                        .Include(w => w.Catalogues)
                        .Include(w => w.Episodes).ThenInclude(c => c.Clients)
                        .Include(w => w.Episodes).ThenInclude(c => c.Catalogues)
                        .Include(w => w.Actors)
                        .Include(w => w.AlternativeTitles)
                        .Include(w => w.Companies)
                        .Include(w => w.Conflicts)
                        .Include(w => w.Countries)
                        .Include(w => w.Directors)
                        .Include(w => w.Distributors)
                        .Include(r => r.Rights)!.ThenInclude(c => c.LanguageRights).ThenInclude(l => l.Language)
                        .Include(r => r.Rights)!.ThenInclude(c => c.ChannelRights).ThenInclude(c => c.Channel)
                        .Include(w => w.Rights)!.ThenInclude(c => c.Type)
                        .Include(w => w.Rights)!.ThenInclude(c => c.Countries)
                        .Include(w => w.Titles)
                        .Include(w => w.Languages)
                        .Include(w => w.Producers)
                        .Include(w => w.ScreenWriters)
                        .Include(w => w.WorksStatusHistory)
                        .Include(w => w.WorksType)
                        .Include(w => w.Languages)
                        .Include(w => w.Mandates).ThenInclude(c => c.MandateType)
                        .AsSplitQuery()
                        .FirstOrDefault(s => s.Id == request.Id);

            if (season == null)
            {
                Logger.LogInformation((int)SeasonFeatureEvent.CopyNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            var newClient = OscarContext.Clients.FirstOrDefault(c => c.Id == request.NewClientID);

            if (newClient == null)
                return Result.Fail<string>("No valid client id supplied for copy");

            if (request.Relinquish)
            {
                Core.Entities.Catalogue newCat = null;

                if (request.NewCatalogueID > 0)
                    newCat = OscarContext.Catalogues.FirstOrDefault(c => c.Id == request.NewCatalogueID);

                WorksHelper.MoveWorks(newClient, newCat, season);

                if (request.CopyOrMoveUnderlyingWorks)
                {
                    if (season.Episodes != null)
                    {
                        foreach (var episode in season.Episodes)
                        {
                            WorksHelper.MoveWorks(newClient, newCat, episode);
                        }
                    }
                }

                Core.Entities.Series newSeries = null;
                if (request.NewSeriesID > 0)
                    newSeries = OscarContext?.Series?.FirstOrDefault(c => c.Id == request.NewSeriesID);
                if (newSeries != null)
                {
                    season.Series = newSeries;
                }
            }
            else
            {
                var newSeries = OscarContext.Series
                            .FirstOrDefault(s => s.Id == request.NewSeriesID);

                var newSeason = WorksHelper.CopySeason(season, OscarContext, request.NewClientID, 
                    request.NewCatalogueID, newSeries, request.CopyOrMoveUnderlyingWorks);

                int counter = 1;
                newSeason.CompactRef = AutoGenerateCompactRef(counter);

                if (newSeason?.Episodes != null)
                {
                    foreach (var newEpisode in newSeason?.Episodes)
                    {
                        counter = counter + 1;
                        newEpisode.CompactRef = AutoGenerateCompactRef(counter);
                    }
                }
                OscarContext.Seasons.Add(newSeason);
            }
            OscarContext.SaveChanges();

            Logger.LogInformation((int)SeasonFeatureEvent.Copy, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }       
    }
}