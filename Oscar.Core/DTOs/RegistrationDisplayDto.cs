namespace Oscar.Core.DTOs
{
    public record RegistrationDisplayDto
    {
        public int Id { get; set; }
        public string? ClientName { get; set; }
        public string? CatalogueName { get; set; }
        //public string? WorksTitle { get; set; }
        public string? SocietyName { get; set; }
        public DateTime? DateRegistered { get; set; }
        public Enums.RegisterType? RegisterType { get; set; }
        public Enums.RegisterStatus? RegisterStatus { get; set; }
        public string? Notes { get; set; }

        public ICollection<WorksTitleDto>? Titles { get; set; }
        public RegistrationBatchDisplayDto RegistrationBatch { get; set; }

        public int SocietyId { get; set; }
        public SocietyDto Society { get; set; }

    }
}