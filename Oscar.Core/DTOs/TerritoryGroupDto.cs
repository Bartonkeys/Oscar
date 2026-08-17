namespace Oscar.Core.DTOs;

public record TerritoryGroupDto
{
    public int Id { get; set; }
    public string GroupCode { get; set; }
    public string GroupName { get; set; }
    public ICollection<TerritoryDto> Territories { get; set; }
}