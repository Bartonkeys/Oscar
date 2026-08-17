using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Core.DTOs
{
    public class RegistrationWorksUpfarArgoaDto
    {
        public string? FileName { get; set; }
        public IEnumerable<UpfarArgoaRow> Rows { get; set; }
    }

    public class UpfarArgoaRow
    {
        public string? SeriesOrStandAloneTitle { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? SeasonTitle { get; set; }
        public string? WorkType { get; set; }
        public string? ProductionCountry { get; set; }
        public string? IdentificationCode { get; set; }
        public string? RightHolder { get; set; }
        public string? Producer { get; set; }
        public string? Performer { get; set; }
        public string? ManagedRights { get; set; }
        public string? QuotaRightsHeld { get; set; }
        public string? Duration { get; set; }
        public string? DateOfRegistration { get; set; }
        public string? YearOfCalculating { get; set; }
        public string? ReciprocalContracts { get; set; }
        public string? Observations { get; set; }
        public string ManagedRightsRetransmission { get; set; } = "No";
        public string ManagedRightsPrivate { get; set; } = "Yes";
        public object ManagedRightsPublic { get; set; } = "Yes";
        public string? RightsFrom { get; set; }
        public string? RightsTo { get; set; }
    }
}
