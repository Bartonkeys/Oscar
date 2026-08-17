using Oscar.Core.Enums;

namespace Oscar.Core.Entities;

public class VwOnMusicFelixWorks
{
    public int ClientsId { get; set; }
    public int CataloguesId { get; set; }
    public string? ClientName { get; set; }
    public string? CatalogueName { get; set; }
    public int WorksId { get; set; }
    public string? WorksReference { get; set; }
    public string As400refNo { get; set; }
    public int? EpisodeRef { get; set; }
    public int? SerialLevel { get; set; }
    public string? CompactRef { get; set; }
    public string? SeriesRef { get; set; }
    public string? SeasonRef { get; set; }
    public int? SeasonNo { get; set; }
    public string WorkType { get; set; }
    public int Documentary { get; set; }
    public int? ProductionYear { get; set; }
    public int? FirstBroadcastYear { get; set; }
    public int? Duration { get; set; }
    public string? Isan { get; set; }
    public string? SeriesTitle { get; set; }
    public string? Titles { get; set; }
    public string? Actors { get; set; }
    public string? Directors { get; set; }
    public string? Producers { get; set; }
    public string? ProductionCompanies { get; set; }
    public string? Genre { get; set; }
    public string? GenreSubType { get; set; }
    public int ClientStatus { get; set; }
    public string ClientGrade { get; set; }
    public string ContractEndDate { get; set; }
    public bool AutoRenewMandate { get; set; }
    public string? Nationality { get; set; }
}

public partial class OnMusicMatch
{
    public int Id { get; set; }
    public int WorksId { get; set; }
    public string? Message { get; set; }
    public int OnMusicMatchStatusId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public int RetryCount { get; set; }

    public virtual OnMusicMatchStatus OnMusicMatchStatus { get; set; }
}

public partial class OnMusicMatchStatus
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<OnMusicMatch> OnMusicMatches { get; set; }
}