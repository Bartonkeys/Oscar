namespace Oscar.Core.DTOs;

public record WorksTitleResponseDto
{
    public int WorksId { get; set; }
    public string? Title { get; set; }
}