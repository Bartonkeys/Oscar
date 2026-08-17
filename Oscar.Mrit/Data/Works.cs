using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Works: BaseEntity
    {
        public int WorksId { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
