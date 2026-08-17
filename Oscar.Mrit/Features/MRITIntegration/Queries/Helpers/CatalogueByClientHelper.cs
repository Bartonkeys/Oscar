using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Oscar.MRIT.Core.DTOs;

namespace Oscar.Mrit.Features.MRITIntegration.Queries.Helpers
{
    internal class CatalogueByClientHelper
    {
        public static IEnumerable<ClientCataloguesDto> GroupClientsIntoCatalogues(IEnumerable<ClientCatalogueQueryObject> clientsAndCatalogues)
        {
            var _catalogueComparer = new CatalogueComparer();

            foreach (var clientGroup in clientsAndCatalogues.GroupBy(c => new { c.ClientsId, c.ClientName }))
            {
                var clientWorksDto = new ClientCataloguesDto
                {
                    ClientId = clientGroup.Key.ClientsId,
                    ClientName = clientGroup.Key.ClientName?.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title) ?? string.Empty,
                    Catalogues = clientGroup.Where(x => x.CataloguesId > 0 && !string.IsNullOrWhiteSpace(x.CatalogueName))
                        .Select(c => new CatalogueDto { CatalogueId = c.CataloguesId, CatalogueName = c.CatalogueName.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title) })
                        .Distinct(_catalogueComparer).ToList()
                };
                yield return clientWorksDto;
            }
        }
    }

    internal class ClientCatalogueQueryObject
    {
        public int ClientsId { get; set; }
        public string ClientName { get; set; }
        public int CataloguesId { get; set; }
        public string CatalogueName { get; set; }
    }

    internal class CatalogueComparer : IEqualityComparer<CatalogueDto>
    {
        public bool Equals(CatalogueDto x, CatalogueDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.CatalogueId == y.CatalogueId && x.CatalogueName == y.CatalogueName;
        }

        public int GetHashCode(CatalogueDto obj)
        {
            return HashCode.Combine(obj.CatalogueId, obj.CatalogueName);
        }
    }
}
