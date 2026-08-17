using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum WorksListColumn
{
    As400Ref,
    Director,
    Actor,
    Producer,
    ScreenWriter,
    AgicoaRef,
    ProductionYear,
    Status,
    Nationality,
    CompactRef,
    WorkType,
    WorksSubType,
    Duration,
    Isan,
    Genre,
    DateCreated,
}
