using System.Collections.Generic;

namespace Oscar.MRIT.Core.MRITModels
{
    public record ProductionNameModel
    {
        public int ProductionId { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
