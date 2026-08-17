using Microsoft.AspNetCore.Http;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record MatchRequestDto
    {
        public int Id { get; set; }
        public MatchRequestStatus Status { get; set; }
        public string Reference { get; set; }
        public string RequestedBy { get; set; }

        public MatchRules Rules { get; set; }

        public int? ClientId { get; set; }
        public int? TerritoryId { get; set; }
        public int? RightsTypeId { get; set; }
        public int? ProductionYear { get; set; }
        public int? RightsFromYear { get; set; }
        public int? RightsToYear { get; set; }

        public string? IgnoreCharactersFollowing { get; set; }
        public string? MatchingResultPublicUrl { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? LastModified { get; set; }

    }
}
