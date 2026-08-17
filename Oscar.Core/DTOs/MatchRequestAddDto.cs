using Microsoft.AspNetCore.Http;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record MatchRequestAddDto
    {
        public MatchRules? Rules { get; set; }
        public IFormFile? FormFile { get; set; }
        public string? RequestedBy { get; set; }
        public int? ClientId { get; set; }
        public int? TerritoryId { get; set; }
        public int? RightsFromYear { get; set; }
        public int? RightsToYear { get; set; }
        public string? IgnoreCharactersFollowing { get; set; }
        public RightsTypeDto? RightsType { get; set; }
    }
}
