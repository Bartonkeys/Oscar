using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record WorksTitleDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? LanguageCode { get; set; }
        public TitleType TitleType { get; set; }
    }

    public static class TitleTypeExtensions
    {
        public static bool In(this TitleType titleType, TitleType[] titleTypes)
        {
            foreach (var type in titleTypes)
            {
                if (type == titleType) return true;
            }
            return false;
        }

        public static bool IsMainTitles(this TitleType titleType)
        {
            return titleType.In(new[] { TitleType.Main, TitleType.Episode });
        }

        public static bool IsAlternateTitles(this TitleType titleType)
        {
            return titleType.In(new[] { TitleType.MainAlternative, TitleType.EpisodeAlternative });
        }
    }
}
