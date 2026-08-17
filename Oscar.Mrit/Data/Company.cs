using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Company : BaseName
    {
        public string SimpleName { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
