using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum ClientListColumn
{
    AltNames,
    EMail,
    ContractFirstStartDate,
    ContractCurrentStartDate,
    ContractEndDate,
    LastModified,
    DateCreated,
    ClientReference
}
