using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Oscar.Mrit.Data
{
    public class Person : BaseEntity
    {
        public string Forename { get; set; }
        public string MiddleNames { get; set; }
        public string Surname { get; set; }

        [StringLength(450)]
        public string SimpleName { get; set; }

        public ICollection<PersonOfInterest> PersonOfInterests { get; set; }
    }
}
