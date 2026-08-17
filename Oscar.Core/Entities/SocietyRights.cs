namespace Oscar.Core.Entities;

public class SocietyRights : BaseEntity
{
    public RightsType RightsType { get; set; }
    public Society Society { get; set; }
    public Country Country { get; set; }
}