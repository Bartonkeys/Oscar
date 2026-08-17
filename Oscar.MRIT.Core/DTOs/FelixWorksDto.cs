using System;
using System.Collections.Generic;
using System.Text;

namespace Oscar.MRIT.Core.DTOs
{
    public class FelixWorksDto
    {
        public int ClientsId { get; set; }
        public int CataloguesId { get; set; }
        public string ClientName { get; set; }
        public string CatalogueName { get; set; }
        public int WorksId { get; set; }
        public string WorksReference { get; set; }
        public string As400refNo { get; set; }
        public int EpisodeRef { get; set; }
        public int SerialLevel { get; set; }
        public string CompactRef { get; set; }
        public string SeriesRef { get; set; }
        public string SeasonRef { get; set; }
        public int SeasonNo { get; set; }
        public string WorkType { get; set; }
        public byte Documentary { get; set; }
        public string ProductionYear { get; set; }
        public string FirstBroadcastYear { get; set; }
        public string Duration { get; set; }
        public string Isanno { get; set; }
        public string SeriesTitle { get; set; }
        public string Titles { get; set; }
        public string Actors { get; set; }
        public string Directors { get; set; }
        public string Producers { get; set; }
        public string ProductionCompanies { get; set; }
        public string ClientStatus { get; set; }
        public string ClientGrade { get; set; }
        public string ContractEndDate { get; set; }
        public string AutoRenewMandate { get; set; }
    }
}
