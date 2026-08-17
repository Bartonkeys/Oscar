namespace Oscar.Core.DTOs
{
    public record CountryDto : LookUpDto
    {
        public string Code { get; set; }
        public string? Code3A { get; set; }
        public bool InUse { get; set; }
    }
}
