namespace Oscar.Core.DTOs
{
    public record CatalogueUpdateDto
    {
        public int id { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string IMaestroClientCode { get; set; }
        public string AgicoaClientRef { get; set; }
        public string? GeneralNotes { get; set; }
        public ICollection<OtherNameDto>? OtherNames { get; set; }
        public ICollection<MandateTypeDto>? MandateTypes { get; set; }

    }
}
