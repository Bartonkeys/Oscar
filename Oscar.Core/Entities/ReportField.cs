using System;
namespace Oscar.Core.Entities
{
	
    public class ReportField : BaseEntity
    {
        public string? BaseEntityName { get; set; }
        public string? ReportFieldName { get; set; }
        public Report Report { get; set; }
        public string? DisplayName { get; set; }

        //NB: Currently only using for 0=standard, 1=enum
        public int DataType { get; set; }
    }
    
}

