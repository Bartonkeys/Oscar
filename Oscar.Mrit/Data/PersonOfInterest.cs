using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class PersonOfInterest : BaseEntity
    {
        public Person Person { get; set; }
        public PersonType PersonType { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
