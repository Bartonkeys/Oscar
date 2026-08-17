using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.MRIT.Core.DTOs
{
    public class WorksDto
    {
        public int WorksId { get; set; }
        public int? WorksCode { get; set; }
        public string WorksReference { get; set; }
        public string As400refNo { get; set; }
        public int SerialLevel { get; set; }
        public int EpisodeRef { get; set; }
        public string CompactRef { get; set; }
        public string SeriesRef { get; set; }
        public string SeasonRef { get; set; }
        public int SeasonNo { get; set; }
        public short EpisodeCount { get; set; }
        public short SeasonCount { get; set; }
        public string WorkType { get; set; }
        public string Genre { get; set; }
        public string GenreSubType { get; set; }
        public byte Colour { get; set; }
        public byte Colorized { get; set; }
        public byte Silent { get; set; }
        public byte Drama { get; set; }
        public byte Comedy { get; set; }
        public byte Documentary { get; set; }
        public string ProductionYear { get; set; }
        public string FirstBroadcastYear { get; set; }
        public string Nationality { get; set; }
        public string Duration { get; set; }
        public string CopyReg { get; set; }
        public string Isanno { get; set; }
        public byte IsReleased { get; set; }
        public byte ModifiedVersion { get; set; }
        public byte ContinuousProduction { get; set; }
        public string WorkStatus { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
