using System;
using Newtonsoft.Json;
using Oscar.Core.Extensions;
namespace Oscar.Core.DTOs
{
    public class EnumDTO
    {
        public int Key => Convert.ToInt32(_enum);
        public string Name => _enum.ToDescription();
        public string FriendlyName => JsonConvert.SerializeObject(_enum).Cleanse();

        public bool Selected { get; set; }

        private readonly Enum _enum;
        public EnumDTO(Enum inputEnum)
        {
            _enum = inputEnum;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
