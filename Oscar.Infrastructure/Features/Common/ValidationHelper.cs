namespace Oscar.Infrastructure.Features.Common
{
    public class ValidationHelper
    {
        public static bool HaveAtLeastOneTitle(ICollection<Core.DTOs.WorksTitleDto>? titles)
        {
            return titles != null && !string.IsNullOrWhiteSpace(titles.FirstOrDefault()?.Title ?? string.Empty);
        }
    }
}