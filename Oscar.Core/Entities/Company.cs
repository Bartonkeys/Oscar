namespace Oscar.Core.Entities;

public class Company: BaseEntity
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

    public string? Email { get; set; }
    public Address? Address { get; set; }

    public ICollection<Works>? Works { get; set; }
}