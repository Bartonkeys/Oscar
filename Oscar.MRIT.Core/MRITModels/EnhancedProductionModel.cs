using System.Collections.Generic;

namespace Oscar.MRIT.Core.MRITModels
{
    public record EnhancedProductionModel
    {
        public int Id { get; set; }
        public IEnumerable<ProductionModel> ProductionModels { get; set; }
    }
}
