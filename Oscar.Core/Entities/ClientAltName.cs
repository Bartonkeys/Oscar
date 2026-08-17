using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Core.Entities
{
    public class ClientAltName: BaseEntity
    {
        private string _altName;
        public string AltName
        {
            get { return _altName; }
            set
            {
                _altName = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
            }
        }

        public AltNameType AltNameType { get; set; }
        public Client Client { get; set; }
    }
}
