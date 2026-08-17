using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Genre : BaseName
    {
        public string Description { get; set; }
        public bool IsApproved { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}
