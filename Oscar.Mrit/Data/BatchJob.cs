using System;
using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class BatchJob: BaseEntity
    {
        public Guid BatchJobKey { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
