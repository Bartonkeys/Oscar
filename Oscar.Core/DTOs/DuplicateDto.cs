namespace Oscar.Core.DTOs
{
    public record DuplicateDto
    {
        public string Title { get; set; }
        public string Client { get; set; }
        public string Catalogue { get; set; }
    }
}