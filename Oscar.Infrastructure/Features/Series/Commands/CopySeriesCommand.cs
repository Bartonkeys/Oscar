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

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class CopySeriesCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public int NewClientID { get; set; }
        public int NewCatalogueID { get; set; }
        public bool CopyOrMoveUnderlyingWorks { get; set; }
        public bool Relinquish { get; set; }
    }

    public class CopySeriesCommandHandler : AbstractBaseHandler<CopySeriesCommand, string>
    {
        public CopySeriesCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<CopySeriesCommand> validator, ILogger<CopySeriesCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(CopySeriesCommand request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                        .Include(w => w.Clients)
                        .Include(w => w.Catalogues)
                        .Include(w => w.Seasons).ThenInclude(c => c.Clients)
                        .Include(w => w.Seasons).ThenInclude(c => c.Catalogues)
                        .Include(w => w.Seasons).ThenInclude(w => w.Episodes).ThenInclude(c => c.Clients)
                        .Include(w => w.Seasons).ThenInclude(w => w.Episodes).ThenInclude(c => c.Catalogues)
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

            if (series == null)
            {
                Logger.LogInformation((int)SeriesFeatureEvent.CopyNotFound, $"Not found {request.Id}");
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

                WorksHelper.MoveWorks(newClient, newCat, series);

                if (request.CopyOrMoveUnderlyingWorks)
                {
                    if (series.Seasons != null)
                    {
                        foreach(var season in series.Seasons)
                        {
                            WorksHelper.MoveWorks(newClient, newCat, season);
                            if (season.Episodes != null)
                            {
                                foreach (var episode in season.Episodes)
                                {
                                    WorksHelper.MoveWorks(newClient, newCat, episode);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var newSeries = WorksHelper.CopySeries(series, OscarContext, request.NewClientID, request.NewCatalogueID, request.CopyOrMoveUnderlyingWorks);

                int counter = 1;
                newSeries.CompactRef = AutoGenerateCompactRef(counter);

                if (newSeries?.Seasons != null)
                {
                    foreach (var newSeason in newSeries?.Seasons)
                    {
                        counter = counter + 1;
                        newSeason.CompactRef = AutoGenerateCompactRef(counter);

                        foreach (var newEpisode in newSeason.Episodes)
                        {
                            counter = counter + 1;
                            newEpisode.CompactRef = AutoGenerateCompactRef(counter);
                        }
                    }
                }

                OscarContext.Series.Add(newSeries);
            }
            OscarContext.SaveChanges();

            Logger.LogInformation((int)SeasonFeatureEvent.Copy, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }
    }
}