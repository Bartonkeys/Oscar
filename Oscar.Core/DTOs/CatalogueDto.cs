using Oscar.Core.Entities;

namespace Oscar.Core.DTOs
{
    public record CatalogueDto
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public string Name { get; set; }
        public string IMaestroClientCode { get; set; }
        public string AgicoaClientRef { get; set; }
        public string? GeneralNotes { get; set; }
        public ICollection<OtherNameDto>? OtherNames { get; set; }
        public ICollection<SocietyDto> Societies { get; set; }
        public ICollection<MandateDto> Mandates { get; set; }
    }
}
