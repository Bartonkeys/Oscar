using Oscar.Core.Enums;

namespace Oscar.Core.Entities;

public class OtherName: BaseEntity
{
    public OtherNameType Type { get; set; }
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
    }
    public Client? Client { get; set; }
    public Catalogue? Catalogue { get; set; }

}