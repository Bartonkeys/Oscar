using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oscar.Core.Entities;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum RegisterType
{
    [EnumMember(Value = "Zero")]
    Zero = 0,

    [EnumMember(Value = "One")]
    One = 1,

    [EnumMember(Value = "Two")]
    Two = 2,

    [EnumMember(Value = "Nine")]
    Nine = 9,

}