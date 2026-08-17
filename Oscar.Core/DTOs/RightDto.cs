
using Oscar.Core.Common;
using Oscar.Core.Entities;
using System.Text.Json.Serialization;

namespace Oscar.Core.DTOs;

public class RightDto
{
    public int Id { get; set; }
    public int TypeId { get;set; }
    [JsonIgnore] //Added this to ignore self referencing when serializing during cloning of RightDto object 
    public RightsTypeDto Type { get; set; }
    public ClientDto Client { get; set; }
    public TerritoryGroupDto TerritoryGroup { get; set; }
    public DateTime StartOfRight { get; set; }
    public DateTime EndOfRight { get; set; }
    public DateTime StartOfValidity { get; set; }
    public DateTime EndOfValidity { get; set; }
    public string Notations { get; set; }
    public bool RightsPerpetuity => EndOfRight == Constants.Rights.Perpetuity;
    public bool ValidityPerpetuity => EndOfValidity == Constants.Rights.Perpetuity;

    public ICollection<ChannelRightsDto> ChannelRights { get; set; }
    public ICollection<LanguageRightsDto> LanguageRights { get; set; }
    public List<CountriesGroupsDto> CountriesGroups { get; set; }
    public List<string> CountryCountryGroup { get; set; } = new List<string>();
    public List<string> CountriesWithinSelectedGroups{ get; set; } = new List<string>();
    public ICollection<CountryDto> Countries { get; set; }

    public WorksDto? Work { get; set; }

    public CatalogueDto? Catalogue { get; set; }
    public decimal? Percentage { get; set; }
 }

public class LanguageRightsDto
{
    public LanguageDto Language { get; set; }
    public decimal? Percentage { get; set; }
}

public class ChannelRightsDto
{
    public ChannelDto Channel { get; set; }
    public ICollection<CountryChannelRightsDto>? CountryRights { get; set; }
}

public class CountryChannelRightsDto
{
    public decimal? ExcludePercentage { get; set; }
    public CountryDto Country { get; set; }
}

public class ChannelDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool InUse { get; set; }
}


