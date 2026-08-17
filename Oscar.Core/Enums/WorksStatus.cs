using Newtonsoft.Json;
namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum WorksStatus
{
    Any = -1,
    Active = 1,
    Uncontrolled = 2,
    Incomplete = 3,
    Relinquished = 4,
    InConflict = 5,
    Duplicate = 6
}