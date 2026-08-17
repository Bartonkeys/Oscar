using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oscar.Core.Entities;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum CommissionedWorkStatus
{
    [EnumMember(Value = "Commissioned")]
    Commissioned = 0,

    [EnumMember(Value = "Non_Commissioned")]
    Non_Commissioned = 1,

    [EnumMember(Value = "Unknown")]
    Unknown = 2
}


