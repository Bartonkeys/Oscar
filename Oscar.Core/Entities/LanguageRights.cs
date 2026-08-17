using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.Entities
{
    public class LanguageRights: BaseEntity
    {
        public Language Language { get; set; }
        public Right Right { get; set; }
        public decimal? Percentage { get; set; }
    }
}
