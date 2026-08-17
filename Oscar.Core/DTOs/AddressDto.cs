namespace Oscar.Core.DTOs
{
    public record AddressDto: IDto
    {
        public int Id { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? AddressLine4 { get; set; }
        public string? PostZipCode { get; set; }
        public string? Country { get; set; }
        public bool? IsCurrent { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }

    public interface IDto
    {
        public int Id { get; set; }
    }
}