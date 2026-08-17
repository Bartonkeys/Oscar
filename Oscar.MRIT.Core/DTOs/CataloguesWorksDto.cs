using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.MRIT.Core.DTOs
{
    public class CatalogueWorksDto
    {
        public int CatalogueId { get; set; }
        public string CatalogueName { get; set; }
        public IEnumerable<int> WorksIds { get; set; }
    }
}
