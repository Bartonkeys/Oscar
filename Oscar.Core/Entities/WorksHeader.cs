using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Oscar.Core.Entities
{
    public class WorksHeader: ITitle
    {
        public int Id { get; set; }
        public int WorksId { get; set; }
        public string? Discriminator { get; set; }
        public string? Title { get; set; }
        public string? CompactRef { get; set; }
        public int? ProductionYear { get; set; }
        public int? FirstBroadcastYear { get; set; }
        public Enums.WorksStatus? WorksStatus { get; set; }
        public string? ClientName { get; set; }
        public string? CatalogueName { get; set; }
        public int ClientId { get; set; }
        public int CatalogueId { get; set; }
        public string? ReverseTitle { get; set; }
    }
}
