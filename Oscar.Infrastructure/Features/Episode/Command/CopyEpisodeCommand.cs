using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Commands
{
    public class CopyEpisodeCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public int NewClientID { get; set; }
        public int NewCatalogueID { get; set; }
        public int? NewSeriesID { get; set; }
        public int? NewSeasonID { get; set; }
        public bool Relinquish { get; set; }
    }

    public class CopyEpisodeCommandHandler : AbstractBaseHandler<CopyEpisodeCommand, string>
    {
        public CopyEpisodeCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<CopyEpisodeCommand> validator, ILogger<CopyEpisodeCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(CopyEpisodeCommand request, CancellationToken cancellationToken)
        {
            var episode = OscarContext.Episodes
                        .Include(w => w.Clients)
                        .Include(w => w.Catalogues)
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

            if (episode == null)
            {
                Logger.LogInformation((int)EpisodeFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            var newClient = OscarContext.Clients.FirstOrDefault(c => c.Id == request.NewClientID);

            if (newClient == null)
                return Result.Fail<string>("No valid client id supplied for copy");

            if (request.Relinquish)
            {
                if (episode.Clients != null)
                    foreach (var client in episode.Clients)
                        episode.Clients?.Remove(client);
                episode.Clients?.Add(newClient);

                Core.Entities.Catalogue newCat = null;
                if (request.NewCatalogueID > 0)
                    newCat = OscarContext?.Catalogues?.FirstOrDefault(c => c.Id == request.NewCatalogueID);
                if (newCat != null)
                {
                    if (episode.Catalogues != null)
                        foreach (var cat in episode.Catalogues)
                            episode.Catalogues?.Remove(cat);
                    episode.Catalogues?.Add(newCat);
                }

                Core.Entities.Series newSeries = null;
                if (request.NewSeriesID > 0)
                    newSeries = OscarContext?.Series?.FirstOrDefault(c => c.Id == request.NewSeriesID);
                if (newSeries != null)
                {
                    episode.Series = newSeries;
                }

                Core.Entities.Season newSeason = null;
                if (request.NewSeasonID > 0)
                    newSeason = OscarContext?.Seasons.FirstOrDefault(c => c.Id == request.NewSeasonID);
                if (newSeason != null)
                {
                    episode.Season = newSeason;
                }
            }
            else
            {
                var newSeries = OscarContext.Series
                            .FirstOrDefault(s => s.Id == request.NewSeriesID);

                var newSeason = OscarContext.Seasons
                            .FirstOrDefault(s => s.Id == request.NewSeasonID);

                var newEpisode = WorksHelper.CopyEpisode(episode, OscarContext, request.NewClientID,
                    request.NewCatalogueID, newSeries, newSeason);

                newEpisode.CompactRef = AutoGenerateCompactRef();

                OscarContext.Episodes.Add(newEpisode);
            }
            OscarContext.SaveChanges();


            Logger.LogInformation((int)EpisodeFeatureEvent.Copy, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }       
    }
}