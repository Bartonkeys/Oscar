using System;
using Azure.Core;

namespace Oscar.Core.Entities
{
	public class Document : BaseEntity
    {
        public string? FileName { get; set; }
        public string? DisplayName { get; set; }
        public Enums.DocumentType DocumentType { get; set; }
        public string? UploadedBy { get; set; }
        public Client? Client { get; set; }
        public Works? Works { get; set; }
    }
}


