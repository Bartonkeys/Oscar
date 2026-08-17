using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public class ClientAltNameDto
    {
        public int Id { get; set; }
        public string AltName { get; set; }
        public AltNameType AltNameType { get; set; }
    }
}
