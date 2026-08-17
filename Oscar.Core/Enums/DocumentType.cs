using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DocumentType
{
    [EnumMember(Value = "Client")]
    [Description("Client")]
    Client = 0,

    [EnumMember(Value = "Works")]
    [Description("Works")]
    Works = 1
}

