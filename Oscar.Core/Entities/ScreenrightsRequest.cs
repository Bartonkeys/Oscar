using System;
using Azure.Core;

namespace Oscar.Core.Entities
{
	public class ScreenrightsRequest : BaseEntity
    {
        public string? FileName { get; set; }
        public string? RequestedBy { get; set; }
        public string? Url { get; set; }
        public Guid? RequestID { get; set; }
        public Enums.ScreenrightsRequestStatus ScreenrightsRequestStatus { get; set; }
    }
}


