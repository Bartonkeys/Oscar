namespace Oscar.Core.DTOs;

public record SeasonDto: WorksDto
{
    public SeriesDto? Series { get; set; }
    public ICollection<EpisodeDto>? Episodes { get; set; }
}