using Azure.Core;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Extensions;
using System.Net;

namespace Oscar.Infrastructure.Features.Common
{
    public static class RightsHelper
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

        public static void SetChannelRights(Right right, ICollection<int>? updateIds, OscarContext context)
        {
            if (updateIds == null) return;
            if (right.ChannelRights == null) right.ChannelRights = new HashSet<ChannelRights>();

            foreach (ChannelRights record in right.ChannelRights)
            {
                if (!updateIds.Any(a => a == record.Channel.Id))
                {
                    right.ChannelRights.Remove(record);
                }
            }

            var channelIdsToAdd = updateIds.Where(id => !right.ChannelRights.Any(e => e.Channel.Id == id)).ToList();

            foreach (var channelId in channelIdsToAdd)
            {
                Oscar.Core.Entities.Channel channel =  context.Channel.First(c => c.Id == channelId);
                var newRecord = new ChannelRights() { Channel = channel, Right = right };
                right.ChannelRights.Add(newRecord);
            }
        }

        public static void SetLanguageRights(Right right, ICollection<int>? updateIds, OscarContext context)
        {
            if (updateIds == null) return;
            if (right.LanguageRights == null) right.LanguageRights = new HashSet<LanguageRights>();

            foreach (LanguageRights record in right.LanguageRights)
            {
                if (!updateIds.Any(a => a == record.Language.Id))
                {
                    right.LanguageRights.Remove(record);
                }
            }

            var languageIdsToAdd = updateIds.Where(id => !right.LanguageRights.Any(e => e.Language.Id == id)).ToList();

            foreach (var languageId in languageIdsToAdd)
            {
                Oscar.Core.Entities.Language language = context.Languages.First(l => l.Id == languageId);
                var newRecord = new LanguageRights() { Language = language, Right = right };
                right.LanguageRights.Add(newRecord);
            }
        }
    }
}
