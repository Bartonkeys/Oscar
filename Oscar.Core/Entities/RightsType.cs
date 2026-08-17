namespace Oscar.Core.Entities;

public class RightsType: LookUpEntity
{
    public ICollection<Right> Rights { get; set; }
}