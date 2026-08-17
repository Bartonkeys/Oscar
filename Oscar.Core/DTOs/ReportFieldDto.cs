using System;
namespace Oscar.Core.DTOs
{
	public class ReportFieldDto
	{
		public string? BaseEntityName { get; set; }
		public string? ReportFieldName { get; set; }
		public string? DisplayName { get; set; }

		//NB: Currently only using for 0=standard, 1=enum
		public int DataType { get; set; }

        public ReportFieldDto()
		{ }

        public ReportFieldDto(string baseEntityName, string reportFieldName, string? displayName = null, int dataType = 0)
		{
			BaseEntityName = baseEntityName;
			ReportFieldName = reportFieldName;
            DisplayName = displayName != null ? displayName : reportFieldName;
			DataType = dataType;
		}

	}

}

