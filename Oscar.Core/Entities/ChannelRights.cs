namespace Oscar.Core.Entities;

public class ChannelRights : BaseEntity
{
    public Channel Channel { get; set; }
    public Right Right { get; set; }

    public ICollection<CountryChannelRights>? CountryRights { get; set; }
}