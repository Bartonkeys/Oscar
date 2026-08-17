namespace Oscar.Core.Entities;

public class CountryGroup: BaseEntity
{
    public string GroupName { get; set; }
    public ICollection<Country> Countries { get; set; }
}