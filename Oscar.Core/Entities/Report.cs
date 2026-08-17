using System.Security.AccessControl;
using Oscar.Core.Enums;

namespace Oscar.Core.Entities
{
	public class Report : BaseEntity
	{
        public string? ReportName { get; set;  }
        public string? BaseEntityName { get; set; }
        public string? ReportUrl { get; set; }
        public ReportStatus ReportStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastRunDate { get; set; }

        public ICollection<ReportField>? ReportFields { get; set; }
    }


}

