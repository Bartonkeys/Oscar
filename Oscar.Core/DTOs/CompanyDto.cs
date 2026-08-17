namespace Oscar.Core.DTOs
{
    public record CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public AddressDto? Address { get; set; }
    }
}