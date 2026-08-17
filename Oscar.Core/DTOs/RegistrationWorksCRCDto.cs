using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksCRCDto
    {
        public string? FileName { get; set; }
        public IEnumerable<CRCRow> Rows { get; set; }
        public string ClientName { get; set; }
    }

    public class CRCRow
    {
        public string? CompactRef { get; set; }
        public string? Name { get; set; }
        public string? FirstStartDate { get; set; }
        public string? EndDate { get; set; }
        public string? OriginalTitle { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? SeasonCount { get; set; }
        public string? EpisodeCount { get; set; }
        public string? AltTitles { get; set; }
        public string? TitleType { get; set; }
        public string? WorkType { get; set; }
        public string? ProductionCompanies { get; set; }
        public string? Directors { get; set; }
        public string? Actors { get; set; }
        public string? ProductionYear { get; set; }
        public string? ProductionCountries { get; set; }
        public string? Duration { get; set; }
        public string? RightsStr { get; set; }
    }
}
