using System.ComponentModel.DataAnnotations;

namespace Oscar.Core.Entities;

public class SocietyReference : BaseEntity
{
    [StringLength(15)]
    public string? Reference { get; set; }

    [StringLength(8)]
    public string? CompactReference { get; set; }

    public Society? Society { get; set; }
    public Works? Works { get; set; }
    public Catalogue? Catalogue { get; set; }
    public Client? Client { get; set; }
}

public class ClientReference : BaseEntity
{
    [StringLength(15)]
    public string? AgicoaDeclarationNumber { get; set; }

    public Works? Works { get; set; }
    public Catalogue? Catalogue { get; set; }
    public Client? Client { get; set; }
}