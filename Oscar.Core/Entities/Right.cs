namespace Oscar.Core.Entities;

public class Right: BaseEntity, IEquatable<Right>
{
    public DateTime StartOfRight { get; set; }
    public DateTime EndOfRight { get; set; }
    public DateTime StartOfValidity { get; set; }
    public DateTime EndOfValidity { get; set; }
    public string? Notations { get; set; }
    public decimal? Percentage { get; set; }
    public DateTime? BulkAmendRights { get; set; }

    public RightsType Type { get; set; }
    public Client? Client { get; set; }
    public Works? Work { get; set; }
    public Catalogue? Catalogue { get; set; }

    public ICollection<ChannelRights> ChannelRights { get; set; }
    public ICollection<LanguageRights> LanguageRights { get; set; }
    public ICollection<Country> Countries { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Right)obj);
    }

    public bool Equals(Right? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Percentage == other.Percentage && Type.Equals(other.Type) && !Countries.Except(other.Countries).Any();
    }

    public override int GetHashCode()
    {
        int countriesHashCode = 0;
        if (Countries != null)
            foreach (var p in Countries)
                countriesHashCode ^= p.GetHashCode();

        return HashCode.Combine(Percentage, Type) ^ countriesHashCode;
    }
}

public class ClientRight : BaseEntity
{
    public DateTime StartOfRight { get; set; }
    public DateTime EndOfRight { get; set; }
    public DateTime StartOfValidity { get; set; }
    public DateTime EndOfValidity { get; set; }
    public string? Notations { get; set; }
    public decimal? Percentage { get; set; }

    public RightsType Type { get; set; }
    public Client? Client { get; set; }
    public Catalogue? Catalogue { get; set; }

    public ICollection<ChannelRights> ChannelRights { get; set; }
    public ICollection<LanguageRights> LanguageRights { get; set; }
    public ICollection<CountryGroup> CountryGroups { get; set; }
    public ICollection<Country> Countries { get; set; }
}