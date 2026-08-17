using Newtonsoft.Json;
namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PersonType
{
    Actor,
    ScreenWriter,
    Director,
    Producer,
    Distributor,
    ScriptWriter
}
