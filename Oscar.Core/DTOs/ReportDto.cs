using System;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
	public class ReportDto 
	{
		public int Id { get; set; }
		public string? ReportName { get; set; }
		public string? BaseEntityName { get; set; }
        public string? ReportUrl { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastRunDate { get; set; }

		public ICollection<ReportFieldDto>? ReportFields { get; set; }
    }
}