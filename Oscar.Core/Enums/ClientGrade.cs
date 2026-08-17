using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oscar.Core.Entities;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum ClientGrade
{
    [EnumMember(Value = "None")]
    None = 0,

    [EnumMember(Value = "Bronze")]
    Bronze = 1,

    [EnumMember(Value = "Silver")]
    Silver = 2,

    [EnumMember(Value = "Gold")]
    Gold = 3,

    [EnumMember(Value = "Platinum")]
    Platinum = 4,

    [EnumMember(Value = "Tin")]
    Tin = 5,

    [EnumMember(Value = "Crossed")]
    Crossed = 6,

    [EnumMember(Value = "Anthem")]
    Anthem = 7
}