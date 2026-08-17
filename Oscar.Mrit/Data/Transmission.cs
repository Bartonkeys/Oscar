using System;
using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Transmission : BaseEntity
    {
        public int MritId { get; set; }
        public string? TransmissionProductionTitle { get; set; }
        public string? TransmissionEpisodeTitle { get; set; }
        public DateTime BroadcastDateTime { get; set; }
        public int BroadcastDuration { get; set; }
        public string? BroadcastLanguage { get; set; }
        public string? Channel { get; set; }
        public ICollection<Territory> Territories { get; set; }
        public Match Match { get; set; }
    }
}