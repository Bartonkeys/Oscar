using System.Xml.Serialization;

namespace Oscar.Core.DTOs
{
    public record WorksImportDto
    {
        public int Id { get; set; }
        public string? WorksType { get; set; }
        public string? SASeriesNumber	{ get; set; }
        public string? SeasonNumber	 { get; set; }
        public string? EpisodeNumber { get; set; }
        public string? Title { get; set; }
        public string? TitleLanguage { get; set; }
        public string? ProductionYear { get; set; }
        public string? Duration { get; set; }
        public string? DirectorFirstName { get; set; }
        public string? DirectorLastName { get; set; }
        public string? ProductionCompany1   { get; set; }
        public string? ProductionCompany2   { get; set; }
        public string? ProductionCompany3   { get; set; }
        public string? ProductionCountry1   { get; set; }
        public string? ProductionCountry2   { get; set; }
        public string? ProductionCountry3   { get; set; }
        public string? ProductionCountry4   { get; set; }
        public string? Actor1FirstName { get; set; }
        public string? Actor1LastName { get; set; }
        public string? Actor2FirstName { get; set; }
        public string? Actor2LastName { get; set; }
        public string? Actor3FirstName { get; set; }
        public string? Actor3LastName { get; set; }
        public string? ExcludedCountries { get; set; }
        public string? ClaimStartDate { get; set; }
        public string? Genre { get; set; }
        public string? WorkSubType { get; set; }
        public string? CAVCOCode { get; set; }
        public string? CrtcCode { get; set; }
        public string? AKATitle1  { get; set; }
        public string? AKATitle1Language { get; set; }
        public string? AKATitle2    { get; set; }
        public string? AKATitle2Language{ get; set; }
        public string? AKATitle3    { get; set; }
        public string? AKATitle3Language{ get; set; }
        public string? Colour { get; set; }
        public string? ValidationMessage { get; set; }
        public bool PossibleDuplicate { get; set; }
        public List<WorksRightsImportDto> WorksRightsImports { get; set; }
    }

    public class WorksRightsImportDto
    {
        public string TypeName { get; set; }
        public string CountryCode { get; set; }
        public string LanguageName { get; set; }
        public string ChannelName { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime StartOfRight { get; set; }
        public DateTime EndOfRight { get; set; }
        public DateTime StartOfValidity { get; set; }
        public DateTime EndOfValidity { get; set; }
    }


    public sealed class WorksImportDtoMap : CsvHelper.Configuration.ClassMap<WorksImportDto>
    {
        public WorksImportDtoMap()
        {
            Map(m => m.WorksType).Name("Works Type");
            Map(m => m.SASeriesNumber).Name("SA/Series #");
            Map(m => m.SeasonNumber).Name("Season #");
            Map(m => m.EpisodeNumber).Name("Episode #");
            Map(m => m.Title).Name("Title");
            Map(m => m.ProductionYear).Name("Production Year");
            Map(m => m.Duration).Name("Duration");
            Map(m => m.DirectorFirstName).Name("Director First Name");
            Map(m => m.DirectorLastName).Name("Director Last Name");
            Map(m => m.ProductionCompany1).Name("Production Company 1");
            Map(m => m.ProductionCompany2).Name("Production Company 2");
            Map(m => m.ProductionCompany3).Name("Production Company 3");
            Map(m => m.ProductionCountry1).Name("Production Country 1");
            Map(m => m.ProductionCountry2).Name("Production Country 2");
            Map(m => m.ProductionCountry3).Name("Production Country 3");
            Map(m => m.ProductionCountry4).Name("Production Country 4");
            Map(m => m.Actor1FirstName).Name("Actor 1 First Name");
            Map(m => m.Actor1LastName).Name("Actor 1 Last Name");
            Map(m => m.Actor2FirstName).Name("Actor 2 First Name");
            Map(m => m.Actor2LastName).Name("Actor 2 Last Name");
            Map(m => m.Actor3FirstName).Name("Actor 3 First Name");
            Map(m => m.Actor3LastName).Name("Actor 3 Last Name");
            Map(m => m.ExcludedCountries).Name("Excluded Countries");
            Map(m => m.ClaimStartDate).Name("Claim Start Date");
            Map(m => m.Genre).Name("Genre");
            Map(m => m.WorkSubType).Name("Work Sub Type");
            Map(m => m.CAVCOCode).Name("CAVCO Code");
            Map(m => m.CrtcCode).Name("CRTC Code");
            Map(m => m.AKATitle1).Name("AKA Title 1");
            Map(m => m.AKATitle1Language).Name("AKA Title 1 Language");
            Map(m => m.AKATitle2).Name("AKA Title 2");
            Map(m => m.AKATitle2Language).Name("AKA Title 2 Language");
            Map(m => m.AKATitle3).Name("AKA Title 3");
            Map(m => m.AKATitle3Language).Name("AKA Title 3 Language");
            Map(m => m.Colour).Name("Colour");
        }
    }

    
}
