using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Territory : BaseName
    {
        public ICollection<Transmission> Transmissions { get; set; }
    }
}