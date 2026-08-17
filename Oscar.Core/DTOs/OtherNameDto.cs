using Oscar.Core.Entities;

namespace Oscar.Core.DTOs
{
    public record OtherNameDto
    {
        public int Id { get; set; }
        public Enums.OtherNameType? Type { get; set; }
        public string Name { get; set; }
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }
    }
}
