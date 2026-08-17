using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record ScreenrightsRequestDto : LookUpDto
    {
        public IFormFile? FormFile { get; set; }
        public string? FileName { get; set; }
        public string? RequestedBy { get; set; }
        public string? Url { get; set; }
        public Guid? RequestID { get; set; }
        public Enums.ScreenrightsRequestStatus ScreenrightsRequestStatus { get; set; }

    }
}