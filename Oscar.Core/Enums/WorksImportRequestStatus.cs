using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum WorksImportRequestStatus
{
    [EnumMember(Value = "None")]
    None,
    [EnumMember(Value = "Pending")]
    Pending,
    [EnumMember(Value = "Processing")]
    Processing,
    [EnumMember(Value = "ValidationFailure")]
    ValidationFailure,
    [EnumMember(Value = "PossibleDuplicates")]
    PossibleDuplicates,
    [EnumMember(Value = "Success")]
    Success,
    [EnumMember(Value = "Error")]
    Error,
    [EnumMember(Value = "RolledBack")]
    RolledBack,
    [EnumMember(Value = "ReSubmit")]
    ReSubmit,
    [EnumMember(Value = "Rollback")]
    Rollback,
    [EnumMember(Value = "ProcessingRollBack")]
    ProcessingRollBack
}