using Microsoft.AspNetCore.Http;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record WorksImportRequestDto
    {
        public int Id { get; set; }
        public WorksImportRequestStatus Status { get; set; }
        public string Reference { get; set; }
        public string RequestedBy { get; set; }

        public int ClientId { get; set; }
        public int? CatalogueId { get; set; }

        public string ClientName { get; set; }
        public string CatalogueName { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public record WorksImportQueueDto
    {
        public int Id { get; set; }
        public WorksImportRequestStatus Status { get; set; }
    }
}
