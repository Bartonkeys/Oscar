namespace Oscar.Core.DTOs
{
    public record TitleLanguageDto
    {
        public int EpisodeNumber { get; set; }
        public string? Title { get; set; }
        public LanguageDto? Language { get; set; }
    }
}
