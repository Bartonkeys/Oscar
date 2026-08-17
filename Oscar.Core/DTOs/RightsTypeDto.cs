namespace Oscar.Core.DTOs;

public record RightsTypeDto: LookUpDto
{
    public ICollection<RightDto> Rights { get; set; }
}