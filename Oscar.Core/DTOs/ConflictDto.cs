namespace Oscar.Core.DTOs
{
    public record ConflictDto
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public DateTime? NotationDate { get; set; }
        public SocietyDto Society { get; set; }
        public int SocietyId { get; set; }
        public string? SocietyName { get; set; }

        public bool Internal { get; set; }
    }
}