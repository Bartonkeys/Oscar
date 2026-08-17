using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class AltProductionTitle : BaseName
    {
        public ICollection<Match> Matches { get; set; }
    }
}