namespace Oscar.Core.DTOs;

public record SeasonUpdateDto: WorksUpdateDto
{
    public int? SeriesId { get; set; }
}