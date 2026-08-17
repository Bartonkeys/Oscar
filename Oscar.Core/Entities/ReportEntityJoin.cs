using System;
namespace Oscar.Core.Entities
{

    public class ReportEntityJoin : BaseEntity
    {
        public string? BaseEntityName { get; set; }
        public string? JoinEntityName { get; set; }
        public string? JoinExpresssion { get; set; }
    }
     
    
}

