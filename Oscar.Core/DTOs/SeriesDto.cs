namespace Oscar.Core.DTOs;

public record SeriesDto : WorksDto
{
    public ICollection<SeasonDto>? Seasons { get; set; }
    public ICollection<EpisodeDto>? Episodes { get; set; }
}