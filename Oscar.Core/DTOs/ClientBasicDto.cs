using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record ClientBasicDto
    {
        public int Id { get; set; }
        public string? ClientName { get; set; }
        public IEnumerable<CatalogueDto>? Catalogues { get; set; }
    }
}
