using System.Collections.Generic;

namespace Oscar.MRIT.Core.MRITModels
{
    public record PersonModel
    {
        public string MainName { get; set; }
        public IEnumerable<string> Names { get; set; }
        public string Type { get; set; }
    }
}
