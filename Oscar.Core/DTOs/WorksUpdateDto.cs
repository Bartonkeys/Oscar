namespace Oscar.Core.DTOs
{
    public record WorksUpdateDto : WorksAddDto
    {
        public ICollection<int>? ClientIds { get; set; }
        public ICollection<int>? CatalogueIds { get; set; }
        public ICollection<int>? RightIds { get; set; }
        public ICollection<int>? ConflictIds { get; set; }
        //public ICollection<int>? CountryIds { get; set; }
        public ICollection<int>? CompanyIds { get; set; }
        public ICollection<int>? AlternativeTitleIds { get; set; }
        public ICollection<int>? ProducerIds { get; set; }
        public ICollection<int>? DirectorIds { get; set; }
        public ICollection<int>? ActorIds { get; set; }
        public ICollection<int>? DistributorIds { get; set; }
        public ICollection<int>? ScreenWriterIds { get; set; }
        public ICollection<int>? ScriptWriterIds { get; set; }
        public ICollection<int>? LanguageIds { get; set; }
        public bool UpdateAllStatus { get; set; }
    }
}
