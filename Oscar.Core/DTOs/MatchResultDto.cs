using Microsoft.AspNetCore.Http;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record MatchResultDto
    {
        public int Id { get; set; }
        public MatchRequestStatus Status { get; set; }
        public string Reference { get; set; }
        public string RequestedBy { get; set; }

        public byte[] FileBytes { get; set; }

    }
}
