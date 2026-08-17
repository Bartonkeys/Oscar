namespace Oscar.Core.DTOs;

public record SeasonAddDto: WorksAddDto
{
    public int? SeriesId { get; set; }
}