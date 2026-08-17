namespace Oscar.Core.Entities;

public class Catalogue : BaseEntity
{
    private string? _name;
    public string? Name
    {
        get { return _name; }
        set
        {
            _name = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
    }

    public string? IMaestroClientCode { get; set; }
    public string? AgicoaClientRef { get; set; }
    public string? GeneralNotes { get; set; }
    public Client Client { get; set; }
    public ICollection<Right>? Rights { get; set; }
    public ICollection<OtherName>? OtherNames { get; set; }
    public ICollection<SocietyReference> SocietyReferences { get; set; }
    public ICollection<Works> Works { get; set; }
    public ICollection<WorksImportRequest> WorksImportRequests { get; set; }
    public ICollection<Mandate> Mandates { get; set; }
    public ICollection<Contact>? Contacts { get; set; }
}