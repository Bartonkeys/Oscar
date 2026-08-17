using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksCCCDto
    {
        public string FileName { get; set; }
        public CCCHeader CccHeader { get; set; }
        public IEnumerable<CCCRow> Rows { get; set; }
        public string ClaimantName { get; set; }
        public string RoyaltyPeriod { get; set; }
        public string ReturnDate { get; set; }
    }

    public class CCCHeader
    {
        public string Year { get; set; }
    }

    public class CCCRow
    {
        public string? ClaimantId { get; set; } 
        public string? ClaimantInternalReferenceNumber { get; set; }
        public string? OwnershipPercentage { get; set; }
        public string? Title { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? Genre { get; set; }
        public string? CopyrightYear { get; set; }
        public string? Country { get; set; }
        public string? Syndication { get; set; }
        public string? Duration { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Broadcast { get; set; }
        public string? PrincipalCast { get; set; }
    }
}
