using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.MRIT.Core.DTOs
{
    public class ClientWorksDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public IEnumerable<int> WorksIds { get; set; }
    }
}
