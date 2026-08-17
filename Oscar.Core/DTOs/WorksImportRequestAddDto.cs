using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record WorksImportRequestAddDto
    {
        public IFormFile? FormFile { get; set; }
        public string RequestedBy { get; set; }

        public int ClientId { get; set; }
        public int? CatalogueId { get; set; }

        public bool IsAgicoa { get; set; }
    }
}
