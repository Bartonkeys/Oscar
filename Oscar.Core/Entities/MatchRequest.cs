using Oscar.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace Oscar.Core.Entities
{
    public class MatchRequest: BaseEntity
    {
        public MatchRequestStatus Status { get; set; }

        [StringLength(30)]
        public string Reference { get; set; }

        [StringLength(30)]
        public string RequestedBy { get; set; }

        public MatchRules Rules { get; set; }

        public int? ClientId { get; set; }
        public int? TerritoryId { get; set; }
        public int? RightsTypeId { get; set; }
        public int? RightsFromYear { get; set; }
        public int? RightsToYear { get; set; }

        [StringLength(200)]
        public string? IgnoreCharactersFollowing { get; set; }
    }

}
