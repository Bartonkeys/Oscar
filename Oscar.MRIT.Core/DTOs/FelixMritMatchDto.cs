using System;
using System.Collections.Generic;

namespace Oscar.MRIT.Core.DTOs
{
    public record FelixMritMatchDto
    {
        public int ProductionId { get; set; }
        public int RecordId { get; set; }
        public List<int> WorksIds { get; set; }
        public string ProductionTitle { get; set; }
        public List<string> AltProductionTitles { get; set; }
        public string RecordTitle { get; set; }
        public List<string> AltRecordTitles { get; set; }
        public List<TransmissionDto> Transmissions { get; set; }
        public int? ProductionYear { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Companies { get; set; }
        public List<string> Languages { get; set; }
        public IList<PersonDto> Actors { get; set; }
        public IList<PersonDto> Directors { get; set; }
        public IList<PersonDto> Producers { get; set; }
        public IList<PersonDto> Writers { get; set; }
        public IList<PersonDto> Creators { get; set; }
        public Guid BatchJobKey { get; set; }
    }

    public record TransmissionDto
    {
        public int MritId { get; set; }
        public string TransmissionProductionTitle { get; set; }
        public string? TransmissionEpisodeTitle { get; set; }
        public DateTime BroadcastDateTime { get; set; }
        public int BroadcastDuration { get; set; }
        public string BroadcastLanguage { get; set; }
        public string Channel { get; set; }
        public List<string> Territories { get; set; }
    }
}