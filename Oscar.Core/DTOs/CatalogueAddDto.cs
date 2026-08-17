using Oscar.Core.Entities;

namespace Oscar.Core.DTOs
{
    public record CatalogueAddDto
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string IMaestroClientCode { get; set; }
        public string? GeneralNotes { get; set; }

    }
}
