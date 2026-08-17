namespace Oscar.Core.DTOs;

public record TerritoryDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string CountryName { get; set; }
    public string CountryCode { get; set; }
}