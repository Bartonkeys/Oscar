using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksEGEDADto
    {
        public string? FileName { get; set; }
        public IEnumerable<EGEDARow> Rows { get; set; }
        public string ClientName { get; set; }
    }

    public class EGEDARow
    {
        public string? CompactRef { get; set; }
        public string? TitleLanguages { get; set; }
        public string? Titles { get; set; }
        public string? SeasonNo { get; set; }
        public string? EpisodeNo { get; set; }
        public string? Duration { get; set; }
        public string? WorkType { get; set; }
        public string? Genre { get; set; }
        public string? YearOfProd { get; set; }
        public string? FirstShowing { get; set; }
        public string? ISANNo { get; set; }
        public string? Colour { get; set; }
        public string? BlackAndWhite { get; set; }
        public string? Silent { get; set; }
        public string? CountriesOfProduction { get; set; }
        public string? OriginalLanguages { get; set; }
        public string? Directors { get; set; }
        public string? Actors { get; set; }
        public string? Producers { get; set; }
        public string? Writers { get; set; }
        public string? ProductionCompanies { get; set; }
        public string? Percentage { get; set; }
        public string? RightsFrom { get; set; }
        public string? RightsTo { get; set; }

    }
}
