using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Oscar.Core.DTOs
{
    public abstract class MatchTemplateDtoBase
    {

        public string? Line { get; set; }
        public string? Title1 { get; set; }
        public string? Title2 { get; set; }
        public string? Title3 { get; set; }
        public string? SeasonNo { get; set; }
        public string? EpisodeNo { get; set; }
        public string? Duration { get; set; }
        public string? ShareAvailable { get; set; }
        public string? Director1 { get; set; }
        public string? Director2 { get; set; }
        public string? ProductionType { get; set; }
        public string? Channel { get; set; }
        public string? BroadcastDate { get; set; } //yyyyMMdd
        public string? ClientReference { get; set; }
        public string? ProductionYear { get; set; }

    }

    public class MatchTemplateDto : MatchTemplateDtoBase
    {
        public string[]? ProductionCountry { get; set; }

        public MatchTemplateDto(string line)
        { this.Line = line; }

        public MatchTemplateDto()
        { }
    }

    

    public sealed class MatchTemplateDtoMap : CsvHelper.Configuration.ClassMap<MatchTemplateDto>
    {
        public MatchTemplateDtoMap()
        {
            Map(m => m.Line).Optional();
            Map(m => m.Title1).Optional();
            Map(m => m.Title2).Optional();
            Map(m => m.Title3).Optional();
            Map(m => m.SeasonNo).Optional();
            Map(m => m.EpisodeNo).Optional();
            Map(m => m.Duration).Optional();
            Map(m => m.ShareAvailable).Optional();
            Map(m => m.ProductionCountry).Convert(args => args.Row.GetField(8).ToString().Split(";"));
            Map(m => m.Director1).Optional();
            Map(m => m.Director2).Optional();
            Map(m => m.ProductionType).Optional();
            Map(m => m.Channel).Optional();
            Map(m => m.BroadcastDate).Optional();
            Map(m => m.ClientReference).Optional();
            Map(m => m.ProductionYear).Optional();
        }
    }
}


