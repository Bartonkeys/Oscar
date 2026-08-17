namespace Oscar.Core.DTOs;

public class RegistrationWorksGWFFDto
{
    public string? FileName { get; set; }
    public IEnumerable<GWFFRow> Rows { get; set; }
    public string ClientName { get; set; }
}

public class GWFFRow
{
    public string? ClientName { get; set; }
    public string? TitleOfSeries { get; set; }
    public string? OriginalTitle { get; set; }
    public string? GermanTitle { get; set; }
    public string? TitleOfEpisodes { get; set; }
    public string? YearOfProduction { get; set; }
    public string? Duration { get; set; }
    public string? TypeOfWork { get; set; }
    public string? Genre { get; set; }
    public string? ProductionCompanies { get; set; }
    public string? Directors { get; set; }
    public string? Actors { get; set; }
    public string? ProductionCountries { get; set; }
    public string? Percentage { get; set; }
    public string? PeriodFrom { get; set; }
    public string? PeriodTo { get; set; }
    public string? SeasonCount { get; set; }
    public string? EpisodeCount { get; set; }
    public string? SeriesCompactNo { get; set; }
    public string? CompactNo { get; set; }
    public string? GWFFNo { get; set; }
    public string? AgicoaNo { get; set; }
    public string? IsanNo { get; set; }
    public string? VamNo { get; set; }
    public string? SuissImageNo { get; set; }

}