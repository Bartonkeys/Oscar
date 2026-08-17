using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class PersonType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PersonOfInterest> PersonOfInterests { get; set; }
    }
}
