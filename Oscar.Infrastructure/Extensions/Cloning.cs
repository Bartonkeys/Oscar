using Azure.Core;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq;

namespace Oscar.Infrastructure.Extensions
{
    public static class Cloning
    {
        public static ICollection<T> Clone<T>(this IEnumerable<T> entities, OscarContext context) where T : BaseEntity, new()
        {
            var results = new List<T>();
            foreach (var entity in entities)
            {
                T newEntity = new();

                var values = context.Entry(entity).CurrentValues.Clone();
                context.Entry(newEntity).CurrentValues.SetValues(values);
                newEntity.Id = 0;

                results.Add(newEntity);
            }

            return results;
        }

        public static ICollection<T> Load<T>(this IEnumerable<T> entities, OscarContext context) where T : BaseEntity, new()
        {
            var results = new List<T>();

            foreach (var entity in entities)
                results.Add(entity);

            return results;
        }

        public static ICollection<Right> CloneRights(this IEnumerable<Right> entities, OscarContext context)
        {
            var results = new List<Right>();
            foreach (var entity in entities)
            {
                Right newEntity = new();

                var values = context.Entry(entity).CurrentValues.Clone();
                context.Entry(newEntity).CurrentValues.SetValues(values);
                newEntity.Id = 0;

                var sourceCountries = context.Countries.Where(c => entity.Countries.Contains(c)).ToList();
                newEntity.Countries = sourceCountries;

                newEntity.Type = context.RightsTypes.Find(entity.Type.Id)!;

                newEntity.LanguageRights = new List<LanguageRights>();
                foreach (var sourceLanguageRight in entity.LanguageRights)
                {
                    var languageRight = new LanguageRights
                    {
                        Language = context.Languages.Find(sourceLanguageRight.Language.Id)!,
                        Right = newEntity
                    };
                    newEntity.LanguageRights.Add(languageRight);
                }

                newEntity.ChannelRights = new List<ChannelRights>();
                foreach (var sourceChannelRight in entity.ChannelRights)
                {
                    var channelRight = new ChannelRights
                    {
                        Channel = context.Channel.Find(sourceChannelRight.Channel.Id)!,
                        Right = newEntity
                    };
                    newEntity.ChannelRights.Add(channelRight);
                }

                results.Add(newEntity);
            }

            return results;
        }

        public static ICollection<Season> CloneSeasons(this IEnumerable<Season> seasons, OscarContext context, int newClientID, int newCatalogueID, Series newSeries, bool copyUnderlyingWorks)
        {
            var results = new List<Season>();

            var seasonIds = seasons.Select(x => x.Id);
            var seasonsWithReferenceData = context.Seasons
                        .Include(w => w.Clients)
                        .Include(w => w.Catalogues)
                        .Include(w => w.Episodes)
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
                        .Where(e => seasonIds.Contains(e.Id));

            foreach (var season in seasonsWithReferenceData)
            {
                var newSeason = WorksHelper.CopySeason(season, context, newClientID, newCatalogueID, newSeries, copyUnderlyingWorks);
                if (season.Episodes != null)
                    newSeason.Episodes = season.Episodes.CloneEpisodes(context, newClientID, newCatalogueID, newSeries, newSeason);
                results.Add(newSeason);
            }

            return results;
        }

        public static ICollection<Episode> CloneEpisodes(this IEnumerable<Episode> episodes, OscarContext context, int newClientID, int newCatalogueID, Series newSeries, Season newSeason)
        {
            var results = new List<Episode>();

            var episodesIds = episodes.Select(x => x.Id);
            var episodesWithReferenceData = context.Episodes
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
                        .Where(e => episodesIds.Contains(e.Id));

            foreach (var episode in episodesWithReferenceData)
            {
                var newEpisode = WorksHelper.CopyEpisode(episode, context, newClientID, newCatalogueID, newSeries, newSeason);
                results.Add(newEpisode);
            }

            return results;
        }

    }
}
