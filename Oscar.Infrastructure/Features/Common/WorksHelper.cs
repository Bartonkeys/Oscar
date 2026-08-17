using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Extensions;

namespace Oscar.Infrastructure.Features.Common
{
    public static class WorksHelper
    {

        public static void SetCollection<T>(ICollection<T>? existingRecords, ICollection<int>? updateIds, OscarContext context) where T : BaseEntity
        {
            if (updateIds == null) return;
            if (existingRecords == null) existingRecords = new HashSet<T>();

            foreach (var record in existingRecords)
            {
                if (!updateIds.Any(a => a == record.Id))
                {
                    existingRecords.Remove(record);
                }
            }

            foreach (var id in updateIds.Where(a => !existingRecords.Any(e => e.Id == a)))
            {
                var newRecord = context.Set<T>().Find(id);

                if (newRecord != null)
                {
                    existingRecords.Add(newRecord);
                }
            }
        }

        public static void SetTitles(ICollection<WorksTitle>? existingRecords, ICollection<WorksTitleDto>? updateRecords, OscarContext context)
        {
            if (updateRecords == null) return;

            if (existingRecords == null) existingRecords = new HashSet<WorksTitle>();

            foreach (var record in existingRecords)
            {
                if (!updateRecords.Any(a => a.Id == record.Id))
                {
                    existingRecords.Remove(record);
                }
                else
                {
                    var updateRecord = updateRecords.FirstOrDefault(u => u.Id == record.Id);
                    if(updateRecord != null)
                    {
                        record.Title = updateRecord.Title;
                        record.LanguageCode = updateRecord.LanguageCode;
                        record.LastModified = DateTime.Now;
                        record.TitleType = updateRecord.TitleType;
                    }
                }
            }

            var recordsToAdd = updateRecords.Where(a => !existingRecords.Any(e => e.Id == a.Id)).ToList();

            foreach (var updateRecord in recordsToAdd)
            {
                var newRecord = new WorksTitle() { LanguageCode = updateRecord.LanguageCode, Title = updateRecord.Title, TitleType = updateRecord.TitleType};
                existingRecords.Add(newRecord);
                
            }
        }

        public static void SetSocieties(ICollection<SocietyReference>? existingRecords, ICollection<SocietyReferenceDto>? updateRecords, OscarContext context)
        {
            if (updateRecords == null) return;

            if (existingRecords == null) existingRecords = new HashSet<SocietyReference>();

            foreach (SocietyReference record in existingRecords)
            {
                if (!updateRecords.Any(a => a.Id == record.Id))
                {
                    existingRecords.Remove(record);
                }
                else
                {
                    var updateRecord = updateRecords.FirstOrDefault(u => u.Id == record.Id);
                    if (updateRecord != null)
                    {
                        Oscar.Core.Entities.Society? society = context.Societies.FirstOrDefault(s => s.Id == updateRecord.SocietyId);
                        record.Society = society;
                        record.Reference = updateRecord.Reference;
                    }
                }
            }

            var recordsToAdd = updateRecords.Where(a => !existingRecords.Any(e => e.Id == a.Id)).ToList();

            foreach (var updateRecord in recordsToAdd)
            {
                Oscar.Core.Entities.Society? society =  context.Societies.FirstOrDefault(s => s.Id == updateRecord.SocietyId);
                var newRecord = new SocietyReference() { Society = society, Reference = updateRecord.Reference };
                existingRecords.Add(newRecord);

            }
        }

        public static void SetClients(ICollection<ClientReference>? existingClientRefs, ICollection<Client>? existingClients, ICollection<ClientReferenceDto>? updateRecords, OscarContext context)
        {
            if (updateRecords == null) return;

            if (existingClientRefs == null) existingClientRefs = new HashSet<ClientReference>();

            foreach (ClientReference record in existingClientRefs)
            {
                if (!updateRecords.Any(a => a.ClientId == record.Id))
                {
                    existingClientRefs.Remove(record);
                }
                else
                {
                    var updateRecord = updateRecords.FirstOrDefault(u => u.ClientId == record.Id);
                    if (updateRecord != null)
                    {
                        Oscar.Core.Entities.Client? client = context.Clients.FirstOrDefault(s => s.Id == updateRecord.ClientId);
                        record.Client = client;
                        record.AgicoaDeclarationNumber = updateRecord.AgicoaDeclarationNumber;
                    }
                }
            }

            var recordsToAdd = updateRecords.Where(a => !existingClientRefs.Any(e => e.Id == a.ClientId)).ToList();

            foreach (var updateRecord in recordsToAdd)
            {
                Oscar.Core.Entities.Client? client = context.Clients.FirstOrDefault(s => s.Id == updateRecord.ClientId);
                var newRecord = new ClientReference() { Client = client, AgicoaDeclarationNumber = updateRecord.AgicoaDeclarationNumber };
                existingClientRefs.Add(newRecord);

            }
            SetCollection<Core.Entities.Client>(existingClients, updateRecords.Select(c => c.ClientId).ToList(), context);
        }

        public static void SetReRegistrations(ICollection<ReRegistration>? existingRecords, ICollection<ReRegistrationDto>? updateRecords, OscarContext context)
        {
            if (updateRecords == null) return;

            if (existingRecords == null) existingRecords = new HashSet<ReRegistration>();

            foreach (var record in existingRecords)
            {
                if (!updateRecords.Any(a => a.Id == record.Id))
                {
                    existingRecords.Remove(record);
                }
                else
                {
                    var updateRecord = updateRecords.FirstOrDefault(u => u.Id == record.Id);
                    if (updateRecord != null)
                    {
                        Core.Entities.Society? society = context.Societies.FirstOrDefault(s => s.Id == updateRecord.Society.Id);
                        record.Society = society;
                    }
                }
            }

            var recordsToAdd = updateRecords.Where(a => !existingRecords.Any(e => e.Id == a.Id)).ToList();

            foreach (var updateRecord in recordsToAdd)
            {
                Core.Entities.Society? society = context.Societies.FirstOrDefault(s => s.Id == updateRecord.Society.Id);
                var newRecord = new ReRegistration() { Society = society };
                existingRecords.Add(newRecord);

            }
        }

        public static void SetMandates(ICollection<Mandate>? existingRecords, ICollection<MandateTypeDto>? updateRecords, OscarContext context)
        {
            if (updateRecords == null) return;

            if (existingRecords == null) existingRecords = new HashSet<Mandate>();

            foreach (var rec in updateRecords)
            {
                var toUpdate = existingRecords.FirstOrDefault(x => x.MandateType.Id == rec.Id);
                if (toUpdate != null)
                {
                    toUpdate.Mandated = rec.Mandated;
                }
                else
                {
                    var newRecord = new Mandate();
                    newRecord.MandateType = context.MandateType.First(x => x.Id == rec.Id);
                    newRecord.Mandated = rec.Mandated;
                    existingRecords.Add(newRecord);
                }
            }
        }

        public static void RemoveCollection<T>(ICollection<T>? childCollection, OscarContext context) where T : BaseEntity
        {
            if (childCollection != null && childCollection.Any())
            {
                context.Set<T>().RemoveRange(childCollection);
            }
        }

        public static void RemoveStandAlone(Core.Entities.StandAlone standAlone, OscarContext context)
        {
            if (standAlone != null)
            {
                WorksHelper.RemoveCollection<WorksTitle>(standAlone.Titles, context);
                WorksHelper.RemoveCollection<Core.Entities.Conflict>(standAlone.Conflicts, context);
                //WorksHelper.RemoveCollection<WorksType>(standAlone.WorksType, context);
                WorksHelper.RemoveCollection<AlternativeTitle>(standAlone.AlternativeTitles, context);
                WorksHelper.RemoveCollection<WorksStatusHistory>(standAlone.WorksStatusHistory, context);
                WorksHelper.RemoveCollection<Company>(standAlone.Companies, context);
                context.StandAlones.Remove(standAlone);
            }
        }

        public static void RemoveSeries(Core.Entities.Series series, OscarContext context)
        {
            if (series != null)
            {
                WorksHelper.RemoveCollection<WorksTitle>(series.Titles, context);
                WorksHelper.RemoveCollection<Core.Entities.Conflict>(series.Conflicts, context);
                //WorksHelper.RemoveCollection<WorksType>(series.WorksType, context);
                WorksHelper.RemoveCollection<AlternativeTitle>(series.AlternativeTitles, context);
                WorksHelper.RemoveCollection<WorksStatusHistory>(series.WorksStatusHistory, context);
                WorksHelper.RemoveCollection<Company>(series.Companies, context);
                RemoveSeasonCollection(series.Seasons, context);

                context.Series.Remove(series);
            }
        }

        public static void RemoveSeason(Core.Entities.Season season, OscarContext context)
        {
            if (season != null)
            {
                var seasonCollection = new List<Core.Entities.Season>();
                seasonCollection.Add(season);
                RemoveSeasonCollection(seasonCollection, context);
            }
        }

        public static void RemoveEpisode(Core.Entities.Episode episode, OscarContext context)
        {
            if (episode != null)
            {
               var episodeCollection = new List<Core.Entities.Episode>();
                episodeCollection.Add(episode);
                RemoveEpisodeCollection(episodeCollection, context);
            }
        }

        public static void MoveWorks(Client? newClient, Core.Entities.Catalogue newCat, Core.Entities.Works work)
        {
            if (work.Clients != null)
            {
                foreach (var client in work.Clients)
                    work.Clients.Remove(client);
            }

            if (work.Catalogues != null && newCat != null)
            {
                foreach (var cat in work.Catalogues)
                    work.Catalogues?.Remove(cat);
            }

            work.Clients?.Add(newClient);

            if (newCat != null)
                work.Catalogues?.Add(newCat);
        }

        public static Core.Entities.Series CopySeries(Core.Entities.Series series, OscarContext context, int newClientID, int newCatalogueID, bool copyUnderlyingWorks)
        {
            Core.Entities.Series newSeries = null;
            if (series != null)
            {
                newSeries = new Core.Entities.Series();

                var values = context.Entry(series).CurrentValues.Clone();
                context.Entry(newSeries).CurrentValues.SetValues(values);
                newSeries.Id = 0;
                
                //do not copy over AS400RefNo
                newSeries.AS400RefNo = null; 

                context.Works.Add(newSeries);

                newSeries.Actors = series.Actors.Load(context);
                newSeries.AlternativeTitles = series.AlternativeTitles.Clone(context);
                newSeries.Companies = series.Companies.Load(context);
                newSeries.Conflicts = series.Conflicts.Clone(context);
                newSeries.Countries = series.Countries.Load(context);
                newSeries.Directors = series.Directors.Load(context);
                newSeries.Distributors = series.Distributors.Load(context);
                newSeries.Rights = series.Rights.CloneRights(context);
                newSeries.Titles = series.Titles.Clone(context);
                newSeries.Languages = series.Languages.Load(context);
                newSeries.Producers = series.Producers.Load(context);
                newSeries.ScreenWriters = series.ScreenWriters.Load(context);
                newSeries.WorksStatusHistory = series.WorksStatusHistory.Clone(context);
                newSeries.Mandates = series.Mandates.Clone(context);

                var newClient = context.Clients.FirstOrDefault(c => c.Id == newClientID);
                if (newClient != null)
                {
                    if (newSeries.Clients == null)
                        newSeries.Clients = new List<Core.Entities.Client>();
                    newSeries.Clients.Add(newClient);
                }

                var newCat = context.Catalogues.FirstOrDefault(c => c.Id == newCatalogueID);
                if (newCat != null)
                {
                    if (newSeries.Catalogues == null)
                        newSeries.Catalogues = new List<Core.Entities.Catalogue>();
                    newSeries.Catalogues.Add(newCat);
                }

                newSeries.ClientReferences = new List<ClientReference> { new ClientReference { Works = newSeries, Client = newClient, Catalogue = newCat }};

                if (copyUnderlyingWorks)
                {
                    newSeries.Seasons = series.Seasons.CloneSeasons(context, newClientID, newCatalogueID, newSeries, copyUnderlyingWorks);
                }
            }
            return newSeries;
        }

        public static Core.Entities.Season CopySeason(Core.Entities.Season season, OscarContext context, int newClientID, int newCatalogueID, Core.Entities.Series newSeries, bool copyUnderlyingWorks)
        {
            Core.Entities.Season newSeason = null;
            if (season != null)
            {
                newSeason = new Core.Entities.Season();

                var values = context.Entry(season).CurrentValues.Clone();
                context.Entry(newSeason).CurrentValues.SetValues(values);
                newSeason.Id = 0;

                //do not copy over AS400RefNo
                newSeason.AS400RefNo = null;

                context.Works.Add(newSeason);

                newSeason.Actors = season.Actors.Load(context);
                newSeason.AlternativeTitles = season.AlternativeTitles.Clone(context);
                newSeason.Companies = season.Companies.Load(context);
                newSeason.Conflicts = season.Conflicts.Clone(context);
                newSeason.Countries = season.Countries.Load(context);
                newSeason.Directors = season.Directors.Load(context);
                newSeason.Distributors = season.Distributors.Load(context);
                newSeason.Rights = season.Rights.CloneRights(context);
                newSeason.Titles = season.Titles.Clone(context);
                newSeason.Languages = season.Languages.Load(context);
                newSeason.Producers = season.Producers.Load(context);
                newSeason.ScreenWriters = season.ScreenWriters.Load(context);
                newSeason.WorksStatusHistory = season.WorksStatusHistory.Clone(context);
                newSeason.Mandates = season.Mandates.Clone(context);

                var newClient = context.Clients.FirstOrDefault(c => c.Id == newClientID);
                if (newClient != null)
                {
                    if (newSeason.Clients == null)
                        newSeason.Clients = new List<Core.Entities.Client>();
                    newSeason.Clients.Add(newClient);
                }

                var newCat = context.Catalogues.FirstOrDefault(c => c.Id == newCatalogueID);
                if (newCat != null)
                {
                    if (newSeason.Catalogues == null)
                        newSeason.Catalogues = new List<Core.Entities.Catalogue>();
                    newSeason.Catalogues.Add(newCat);
                }

                newSeason.ClientReferences = new List<ClientReference> { new ClientReference { Works = newSeason, Client = newClient, Catalogue = newCat } };
                newSeason.Series = newSeries;

                if (copyUnderlyingWorks)
                {
                    if (season.Episodes != null)
                        newSeason.Episodes = season.Episodes.CloneEpisodes(context, newClientID, newCatalogueID, newSeries, newSeason);
                }
            }
            return newSeason;
        }

        public static Core.Entities.Episode CopyEpisode(Core.Entities.Episode episode, OscarContext context, int newClientID, int newCatalogueID, Core.Entities.Series newSeries, Core.Entities.Season newSeason)
        {
            Core.Entities.Episode newEpisode = null;
            if (episode != null)
            {
                newEpisode = new Core.Entities.Episode();

                var values = context.Entry(episode).CurrentValues.Clone();
                context.Entry(newEpisode).CurrentValues.SetValues(values);
                newEpisode.Id = 0;

                //do not copy over AS400RefNo
                newEpisode.AS400RefNo = null;

                if (newEpisode != null)
                {
                    newEpisode.Actors = episode.Actors.Load(context);
                    newEpisode.AlternativeTitles = episode.AlternativeTitles.Clone(context);
                    newEpisode.Companies = episode.Companies.Load(context);
                    newEpisode.Conflicts = episode.Conflicts.Clone(context);
                    newEpisode.Countries = episode.Countries.Load(context);
                    newEpisode.Directors = episode.Directors.Load(context);
                    newEpisode.Distributors = episode.Distributors.Load(context);
                    newEpisode.Rights = episode.Rights.CloneRights(context);
                    newEpisode.Titles = episode.Titles.Clone(context);
                    newEpisode.Languages = episode.Languages.Load(context);
                    newEpisode.Producers = episode.Producers.Load(context);
                    newEpisode.ScreenWriters = episode.ScreenWriters.Load(context);
                    newEpisode.WorksStatusHistory = episode.WorksStatusHistory.Clone(context);
                    //cloneWork.WorksType = work.WorksType.Clone(OscarContext);
                    newEpisode.Mandates = episode.Mandates.Clone(context);

                    var newClient = context.Clients.FirstOrDefault(c => c.Id == newClientID);
                    if (newClient != null)
                    {
                        if (newEpisode.Clients == null)
                            newEpisode.Clients = new List<Core.Entities.Client>();
                        newEpisode.Clients.Add(newClient);
                    }

                    var newCat = context.Catalogues.FirstOrDefault(c => c.Id == newCatalogueID);
                    if (newCat != null)
                    {
                        if (newEpisode.Catalogues == null)
                            newEpisode.Catalogues = new List<Core.Entities.Catalogue>();
                        newEpisode.Catalogues.Add(newCat);
                    }

                    newEpisode.ClientReferences = new List<ClientReference> { new ClientReference { Works = newEpisode, Client = newClient, Catalogue = newCat } };
                    newEpisode.Series = newSeries;
                    newEpisode.Season = newSeason;

                }
            }
            return newEpisode;
        }

        private static void RemoveEpisodeCollection(ICollection<Core.Entities.Episode>? episodeCollection, OscarContext context)
        {
            if(episodeCollection != null && episodeCollection.Any())
            {
                foreach (var episode in episodeCollection)
                {
                    episode.Actors?.Clear();
                    episode.Companies?.Clear();
                    episode.Conflicts?.Clear();
                    episode.AlternativeTitles?.Clear();
                    episode.WorksStatusHistory?.Clear();
                    episode.Rights?.Clear();
                    episode.Catalogues?.Clear();
                    episode.Countries?.Clear();
                    episode.Directors.Clear();
                    episode.Languages.Clear();
                    episode.Clients?.Clear();
                    episode.Producers?.Clear();
                }
                WorksHelper.RemoveCollection<Core.Entities.Episode>(episodeCollection, context);
            }
        }

        private static void RemoveSeasonCollection(ICollection<Core.Entities.Season>? seasonCollection, OscarContext context)
        {
            if (seasonCollection != null && seasonCollection.Any())
            {
                foreach (var season in seasonCollection)
                {
                    season.Actors?.Clear();
                    season.Companies?.Clear();
                    season.Conflicts?.Clear();
                    season.AlternativeTitles?.Clear();
                    season.WorksStatusHistory?.Clear();
                    season.Rights?.Clear();
                    season.Catalogues?.Clear();
                    season.Countries?.Clear();
                    season.Directors.Clear();
                    season.Languages.Clear();
                    season.Clients?.Clear();
                    season.Producers?.Clear();

                    RemoveEpisodeCollection(season.Episodes, context);
                }
                WorksHelper.RemoveCollection<Core.Entities.Season>(seasonCollection, context);
            }
        }
    }
}
