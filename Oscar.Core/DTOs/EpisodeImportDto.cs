namespace Oscar.Core.DTOs
{
    public record EpisodeImportDto
    {
        public string OscarSeriesRef { get; set; } = "";
        public string OscarSeasonRef	 { get; set; } = "";
        public string EpisodeNumber	 { get; set; } = "";
        public string EpisodeTitle { get; set; } = "";
        public string ValidationMessage { get; set; } = "";
    }


    public sealed class EpisodeImportDtoMap : CsvHelper.Configuration.ClassMap<EpisodeImportDto>
    {
        public EpisodeImportDtoMap()
        {
            Map(m => m.OscarSeriesRef).Name("OSCAR Series Ref");
            Map(m => m.OscarSeasonRef).Name("OSCAR Season Ref");
            Map(m => m.EpisodeNumber).Name("Episode Number");
            Map(m => m.EpisodeTitle).Name("Episode Title");
        }
    }

    
}
