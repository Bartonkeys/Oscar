namespace Oscar.Core.DTOs
{
    public record ContactDto: PersonDto
    {
        public string? Title { get; set; }

        public string? Salutation { get; set; }

        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? Comments { get; set; }

        public AddressDto? Address { get; set; }

        public string? Website { get; set; }

        public string? Type { get; set; }

        public string? JobTitle { get; set; }

    }

}