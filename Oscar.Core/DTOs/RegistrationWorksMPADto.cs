using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksMPADto
    {
        public string? FileName { get; set; }
        public IEnumerable<MPARow> Rows { get; set; }
        public string ClientName { get; set; }
        public string VendorId { get; set; }
    }

    public class MPARow
    {
        public string? ClaimantId { get; set; }
        public string? ReferenceId { get; set; }
        public string? CableNetwork { get; set; }
        public string? CableSyndicated { get; set; }
        public string? SatelliteNetwork { get; set; }
        public string? SatelliteSyndicated { get; set; }
        public string? Title { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? Genre { get; set; }
        public string? ProductionYear { get; set; }
        public string? CountryIfNotUS { get; set; }
        public string? DurationMinutes { get; set; }
        public string? ClaimStartDate { get; set; }
        public string? ClaimEndDate { get; set; }
        public string? Cast { get; set; }
        public string? Isan { get; set; }
        public string? Eidr { get; set; }
    }
}
