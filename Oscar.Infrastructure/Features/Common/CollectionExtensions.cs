using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
            {
                @this.Add(element);
            }
        }

        public static IEnumerable<RightDto> GetClientOnlyRights(this IEnumerable<RightDto> inheritedWorksRights)
        {
            if (!inheritedWorksRights.Any())
                return inheritedWorksRights;

            var clientCatalogueId = inheritedWorksRights.Where(r => r.Catalogue != null).Min(r => r.Catalogue.Id);
            return inheritedWorksRights.Where(r => r.Catalogue != null && r.Catalogue.Id == clientCatalogueId);
        }
    }
}
