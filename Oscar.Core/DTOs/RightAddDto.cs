namespace Oscar.Core.DTOs
{
    public record RightAddDto
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int ClientID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime StartValidity { get; set; }
        public DateTime EndValidity { get; set; }
        public string? Notations { get; set; }
        public int? CatalogueID { get; set; }
        public DateTime Creation { get; set; }
        public DateTime? Modified { get; set; }
        public int? WorksID { get; set; }
        public decimal? Percentage { get; set; }
        public ICollection<int>? ChannelIds { get; set; }
        public ICollection<int>? LanguageIds { get; set; }
        public ICollection<int>? CountryIds { get; set; }
    }
}