using System;
namespace Oscar.Core.DTOs
{
	public class ReportDataDto
	{
		public List<Object>? ReportData { get; set; }

		public ReportDataDto()
		{
			ReportData = new List<object>();
		}

		public ReportDataDto(List<Object> reportData)
        {
			this.ReportData = reportData;
        }

		public ReportDataDto(Object reportDataItem)
		{
			this.ReportData = new List<object>();
			ReportData.Add(reportDataItem);
		}

		public void AddReportDataItem(Object reportDataItem)
        {
			ReportData.Add(reportDataItem);

		}

	}
}

