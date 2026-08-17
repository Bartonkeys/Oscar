 namespace Oscar.Core.Entities;

public class Country: LookUpEntity, IEquatable<Country>
{
    public string Code { get; set; }
    public string? Code3A { get; set; }
    public bool InUse { get; set; }
    public ICollection<Works>? Works { get; set; }
    public ICollection<Right>? Rights { get; set; }
    public ICollection<CountryGroup>? CountryGroups { get; set; }

    public bool Equals(Country? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code && Code3A == other.Code3A;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Country)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Code3A);
    }
}