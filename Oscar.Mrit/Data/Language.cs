using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Language : BaseEntity
    {
        public string ISO639_2 { get; set; }
        public string ISO639_1 { get; set; }
        public string EnglishName { get; set; }
        public string FrenchName { get; set; }
        public string GermanName { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
