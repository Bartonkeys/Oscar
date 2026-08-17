using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.MRIT.Core.DTOs
{
    public class ClientCataloguesDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public IEnumerable<CatalogueDto> Catalogues { get; set; }
    }

    public class CatalogueDto
    {
        public int CatalogueId { get; set; }
        public string CatalogueName { get; set; }
    }
}
