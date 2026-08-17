namespace Oscar.Core.DTOs
{
    public record ReRegistrationDto : IDto
    {
        public int Id { get; set; }
        public SocietyDto Society { get; set; }
    }
}