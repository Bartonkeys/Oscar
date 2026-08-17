using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksCMCDto
    {
        public string? FileName { get; set; }
        public IEnumerable<CMCRow> Rows { get; set; } = new List<CMCRow>();
    }

    public class CMCRow
    {
        public string? RHID { get; set; }
        public string? CMCID { get; set; }
        public string? Type { get; set; }
        public string? Genre { get; set; }
        public string? Duration { get; set; }
        public string? ISAN { get; set; }
        public string? EIDR { get; set; }
        public string? YearOfProduction { get; set; }
        public string? OriginalTitleLanguage { get; set; }
        public string? OriginalTitle { get; set; }
        public string? AlternativeTitleLanguage { get; set; }
        public string? AlternativeTitle { get; set; }
        public string? SerialOriginalTitleLanguage { get; set; }
        public string? SerialOriginalTitle { get; set; }
        public string? SerialAlternativeTitleLanguage { get; set; }
        public string? SerialAlternativeTitle { get; set; }
        public string? SerialLevel { get; set; }
        public string? SeasonNumber { get; set; }
        public string? EpisodeNumber { get; set; }
        public string? Director1FirstName { get; set; }
        public string? Director1LastName { get; set; }
        public string? Director2FirstName { get; set; }
        public string? Director2LastName { get; set; }
        public string? Writer1FirstName { get; set; }
        public string? Writer1LastName { get; set; }
        public string? Writer2FirstName { get; set; }
        public string? Writer2LastName { get; set; }
        public string? Actor1FirstName { get; set; }
        public string? Actor1LastName { get; set; }
        public string? Actor2FirstName { get; set; }
        public string? Actor2LastName { get; set; }
        public string? Actor3FirstName { get; set; }
        public string? Actor3LastName { get; set; }
        public string? ProductionCountry1 { get; set; }
        public string? ProductionCountry2 { get; set; }
        public string? ProductionCountry3 { get; set; }
        public string? OriginalLanguage { get; set; }
        public string? ProductionCompany1 { get; set; }
        public string? ProductionCompany2 { get; set; }
        public string? ProductionCompany3 { get; set; }
        public string? Delete { get; set; }
        public string? Tags { get; set; }
        public string? RightsStartDate { get; set; }
        public string? RightsEndDate { get; set; }
    }
}
