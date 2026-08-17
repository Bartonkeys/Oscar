using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oscar.Core.Entities;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum EquivalenceRequestStatus
{
    [EnumMember(Value = "Unprocessed")]
    [Description("Unprocessed")]
    Unprocessed = 0,

    [EnumMember(Value = "Scheduled")]
    [Description("Scheduled")]
    Scheduled = 1,

    [EnumMember(Value = "Processing")]
    [Description("Processing")]
    Processing = 2,

    [EnumMember(Value = "Processed")]
    [Description("Processed")]
    Processed = 3,

    [EnumMember(Value = "Failed")]
    [Description("Failed")]
    Failed = 4,
}
