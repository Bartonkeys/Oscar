namespace Oscar.Core.DTOs;

public record EpisodeUpdateDto: WorksUpdateDto
{
    public int? SeasonId { get; set; }
    public int? SeriesId { get; set; }
}