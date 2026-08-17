namespace Oscar.Core.Entities;

public class Society: BaseEntity
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
    public string? ExpandedName { get; set; }
    public string? GeneralNotes { get; set; }
    public bool? IsClientRegistration { get; set; }
    public bool? IsWorksRegistration { get; set; }
    public string? ClientRegistrationNotes { get; set; }
    public string? WorksRegistrationNotes { get; set; }
    public string? Website { get; set; }

    public ICollection<Address>? Addresses { get; set; }
    public ICollection<Contact>? Contacts { get; set; }
    public ICollection<SocietyReference>? SocietyReferences { get; set; }
    public ICollection<Client>? Clients { get; set; }
    public ICollection<SocietyRights>? SocietyRights { get; set; }
}