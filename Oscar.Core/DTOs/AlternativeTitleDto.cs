namespace Oscar.Core.DTOs
{
    public record AlternativeTitleDto : LookUpDto
    {
        public LanguageDto Language { get; set; }
    }
}

