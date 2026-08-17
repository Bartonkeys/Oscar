namespace Oscar.Core.DTOs
{
    public record WorksAddDto
    {
        public Enums.WorksStatus? WorksStatus { get; set; }
        public string? UncontrolledReason { get; set; }
        public Enums.CommissionedWorkStatus? CommissionedWorkStatus { get; set; }
        public int? GenreId { get; set; }
        public int? DurationMinutes { get; set; }
        public int? ProductionYear { get; set; }
        public int? FirstBroadcastYear { get; set; }
        public string? IMaestroWorkCode { get; set; }
        //public string? AgicoaDeclarationNumber { get; set; }
        public string? AgicoaWorksReference { get; set; }
        public string? Isan { get; set; }
        public string? CompactRef { get; set; }
        public string? AS400RefNo { get; set; }
        public string? CavcoCode { get; set; }
        public string? CrtcCode { get; set; }
        public string? GeneralNotes { get; set; }
        public int? Number { get; set; }
        public int? WorksTypeId { get; set; }
        public int? GenreSubTypeId { get; set; }
        public int? WorksSubTypeId { get; set; }
        public ICollection<WorksTitleDto>? Titles { get; set; }
        public ICollection<SocietyReferenceDto>? SocietyReferences { get; set; }
        public ICollection<ClientReferenceDto>? ClientReferences { get; set; }
        public ICollection<int>? CountryIds { get; set; }
        public ICollection<int>? DirectorIds { get; set; }
        public ICollection<LanguageDto>? Languages { get; set; }
        public ICollection<int>? LanguageIds { get; set; }
        public ICollection<int>? ActorIds { get; set; }
        public ICollection<int>? CompanyIds { get; set; }
        public ICollection<int>? ProducerIds { get; set; }
        public ICollection<ReRegistrationDto>? ReRegistrations { get; set; }
        public ICollection<MandateTypeDto>? MandateTypes { get; set; }
        public ICollection<int>? ClientIds { get; set; }
        public ICollection<int>? CatalogueIds { get; set; }

    }
}
