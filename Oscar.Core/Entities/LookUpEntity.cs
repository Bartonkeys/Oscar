namespace Oscar.Core.Entities;

public class LookUpEntity: BaseEntity, IEquatable<LookUpEntity>
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
    }

    public string? Description { get; set; }

    public bool Equals(LookUpEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Description == other.Description;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LookUpEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description);
    }
}