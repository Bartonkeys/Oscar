using Oscar.Core.Entities;

namespace Oscar.Core.DTOs
{
    public class MandateDto
    {
        public int Id { get; set; }
        public MandateTypeDto MandateType { get; set; }
        public bool Mandated { get; set; }
        public ClientDto Client { get; set; }
        public int ClientId { get; set; }
        public CatalogueDto Catalogue { get; set; }
        public int CatalogueId { get; set; }
        public WorksDto Works { get; set; }
        public int WorksId { get; set; }
    }
}