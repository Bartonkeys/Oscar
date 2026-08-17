using System;
using CsvHelper;
namespace Oscar.Core.DTOs
{
    public class MatchTemplateResultsDto : MatchTemplateDtoBase
    {
        public string? ProductionCountry { get; set; }
        public string? OscarClient { get; set; }
        public string? ClientEndDate { get; set; }
        public string? OscarDirector { get; set; }
        public string? OscarProductionYear { get; set; }
        public string? MatchingIssue { get; set; }

        public MatchTemplateResultsDto()
        { }
    }

    public sealed class MatchTemplateResultsMap : CsvHelper.Configuration.ClassMap<MatchTemplateResultsDto>
    {
        public MatchTemplateResultsMap()
        {
            Map(m => m.Line);
            Map(m => m.Title1).Optional();
            Map(m => m.Title2).Optional();
            Map(m => m.Title3).Optional();
            Map(m => m.SeasonNo).Optional();
            Map(m => m.EpisodeNo).Optional();
            Map(m => m.Duration).Optional();
            Map(m => m.ShareAvailable).Optional();
            Map(m => m.ProductionCountry).Optional();
            Map(m => m.Director1).Optional();
            Map(m => m.Director2).Optional();
            Map(m => m.ProductionType).Optional();
            Map(m => m.Channel).Optional();
            Map(m => m.BroadcastDate).Optional();
            Map(m => m.ClientReference).Optional();
            Map(m => m.OscarClient).Optional();
            Map(m => m.ClientEndDate).Optional();
            Map(m => m.OscarDirector).Optional();
            Map(m => m.OscarProductionYear).Optional();
            Map(m => m.MatchingIssue).Optional();
        }
    }
}

