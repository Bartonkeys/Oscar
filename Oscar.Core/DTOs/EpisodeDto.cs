namespace Oscar.Core.DTOs;

public record EpisodeDto : WorksDto
{
    public SeasonDto? Season { get; set; }
    public SeriesDto? Series { get; set; }
}