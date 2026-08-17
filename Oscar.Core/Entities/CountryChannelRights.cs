namespace Oscar.Core.Entities;

public class CountryChannelRights: BaseEntity
{
    public decimal? ExcludePercentage { get; set; }
    public Country Country { get; set; }
}