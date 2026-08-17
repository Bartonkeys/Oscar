using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;

namespace Oscar.Core.DTOs
{
    public record DocumentDto : LookUpDto
    {
        public string? FileName { get; set; }
        public string? DisplayName { get; set; }
        public Enums.DocumentType DocumentType { get; set; }
        public string? UploadedBy { get; set; }
        public IFormFile? FormFile { get; set; }
        public string? PublicUrl { get; set; }
        public int? OwnerId { get; set; }
      
    }
}
