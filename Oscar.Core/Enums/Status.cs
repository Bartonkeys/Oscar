using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum Status
{
    [EnumMember(Value = "Active_In_Term")]
    Active_In_Term = 1,

    [EnumMember(Value = "Active_Lapsed")]
    Active_Lapsed = 2,

    [EnumMember(Value = "Passive")]
    Passive = 3,

    [EnumMember(Value = "NACC")]
    NACC = 4,

    [EnumMember(Value = "Terminated")]
    Terminated = 5,

    [EnumMember(Value = "Active_Consolidated")]
    Active_Consolidated = 6,

    [EnumMember(Value = "In_Administration")]
    In_Administration = 7,

    [EnumMember(Value = "Terminated_NFC")]
    Terminated_NFC = 8,

    [EnumMember(Value = "Dissolved")]
    Dissolved = 9
}

