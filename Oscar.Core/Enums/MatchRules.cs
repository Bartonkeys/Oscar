using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Oscar.Core.Enums;

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
[Flags]
public enum MatchRules
{
    [EnumMember(Value = "Default")]
    None = 1 << 0,

    //[EnumMember(Value = "Territory")]
    //Territory = 1 << 1,

    [EnumMember(Value = "Match by Production Year")]
    ProductionYear = 1 << 2,

    [EnumMember(Value = "Match by Series Title")]
    SeriesTitle = 1 << 3,

    [EnumMember(Value = "Match by Episode Title")]
    EpisodeTitle = 1 << 4,

    [EnumMember(Value = "Match by Rights Years")]
    RightsYears = 1 << 5,

    [EnumMember(Value = "Match by Rights Type")]
    RightsType = 1 << 6,

    [EnumMember(Value = "Match by Rights Country")]
    RightsCountry = 1 << 7,

    [EnumMember(Value = "Ignore Characters Following")]
    IgnoreCharactersFollowing = 1 << 8,

    [EnumMember(Value = "Match by Director")]
    Director = 1 << 9,

    [EnumMember(Value = "Include Title 1 Check")]
    TitleCheckLevel1 = 1 << 10,

    [EnumMember(Value = "Include Title 2 Check")]
    TitleCheckLevel2 = 1 << 11,

    [EnumMember(Value = "Include Title 3 Check")]
    TitleCheckLevel3 = 1 << 12,

    [EnumMember(Value = "Match by Duration")]
    Duration = 1 << 13,

    [EnumMember(Value = "Match by Production Country")]
    ProductionCountry = 1 << 14,

    [EnumMember(Value = "Match by First Broadcast Year")]
    FirstBroadcastYear = 1 << 15,
}