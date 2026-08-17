namespace Oscar.Core.DTOs;

public record EpisodeAddDto: WorksAddDto
{
    public int? SeasonId { get; set; }
    public int? SeriesId { get; set; }
}