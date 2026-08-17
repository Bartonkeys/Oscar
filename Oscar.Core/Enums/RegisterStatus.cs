using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oscar.Core.Entities;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum RegisterStatus
{
    [EnumMember(Value = "Unregistered")]
    [Description("Unregistered")]
    Unregistered = 0,

    [EnumMember(Value = "Batch_Created")]
    [Description("Batch Created")]
    Batch_Created = 1,

    [EnumMember(Value = "Scheduled")]
    [Description("Scheduled")]
    Scheduled = 2,

    [EnumMember(Value = "Processing")]
    [Description("Processing")]
    Processing = 3,

    [EnumMember(Value = "Registered")]
    [Description("Registered")]
    Registered = 4,

    [EnumMember(Value = "Failed")]
    [Description("Failed")]
    Failed = 5,

    [EnumMember(Value = "Error")]
    [Description("Error")]
    Error = 6,

    [EnumMember(Value = "Errors_Within_Batch")]
    [Description("Errors Within Batch")]
    Errors_Within_Batch = 7,

    [EnumMember(Value = "Batch_Complete")]
    [Description("Batch Complete")]
    Batch_Complete = 8,

    [EnumMember(Value = "Batch_Export_Failed")]
    [Description("Batch Export Failed")]
    Batch_Export_Failed = 9,

    [EnumMember(Value = "Batch_Export_Success")]
    [Description("Batch Export Success")]
    Batch_Export_Success = 10,

    [EnumMember(Value = "UserSelected")]
    [Description("User Selected")]
    UserSelected = 11

}
