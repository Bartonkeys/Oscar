using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class AltRecordTitle : BaseName
    {
        public ICollection<Match> Matches { get; set; }
    }
}