using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Country : BaseName
    {
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public int NumericCode { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}
