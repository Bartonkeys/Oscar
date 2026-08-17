using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Oscar.Core.Entities;

public class Works : BaseEntity
{
    public Enums.WorksStatus? WorksStatus { get; set; }
    public Enums.CommissionedWorkStatus? CommissionedWorkStatus { get; set; }
    public string? UncontrolledReason { get; set; }

    public Genre? Genre { get; set; }
    public int? DurationMinutes { get; set; }
    public int? ProductionYear { get; set; }
    public int? FirstBroadcastYear { get; set; }
    public string? IMaestroWorkCode { get; set; }
    public string? AgicoaWorksReference { get; set; }
    public string? Isan { get; set; }
    [StringLength(15)]
    public string? CavcoCode { get; set; }
    [StringLength(10)] 
    public string? CrtcCode { get; set; }
    public string? GeneralNotes { get; set; }
    public int? Number { get; set; }
    public int? GenreId { get; set; }
    public int? GenreSubTypeId { get; set; }
    public int? WorksSubTypeId { get; set; }
    public int? WorksTypeId { get; set; }
    public string? Discriminator { get; set; }

    [StringLength(15)]
    public string? ColourFormat { get; set; }

    [StringLength(15)]
    public string? CompactRef { get; set; }
    [StringLength(20)]
    public string? AS400RefNo { get; set; }
    [StringLength(20)]
    public string? Nationality { get; set; }

    public int? WorksImportRequestId { get; set; }
    public WorksImportRequest? WorksImportRequest { get; set; }

    public WorksType? WorksType { get; set; }
    public WorksSubType? WorksSubType { get; set; }
    public GenreSubType? GenreSubType { get; set; }
    public ICollection<Client>? Clients { get; set; }
    public ICollection<Catalogue>? Catalogues { get; set; }
    public ICollection<WorksTitle>? Titles { get; set; }
    public ICollection<Right>? Rights { get; set; }
    public ICollection<Conflict>? Conflicts { get; set; }
    public ICollection<Country>? Countries { get; set; }
    public ICollection<Company>? Companies { get; set; }
    public ICollection<AlternativeTitle>? AlternativeTitles { get; set; }
    public ICollection<Producer>? Producers { get; set; }
    public ICollection<Director> Directors { get; set; }
    public ICollection<Actor>? Actors { get; set; }
    public ICollection<Distributor>? Distributors { get; set; }
    public ICollection<ScreenWriter>? ScreenWriters { get; set; }
    public ICollection<ScriptWriter>? ScriptWriters { get; set; }
    public ICollection<WorksStatusHistory> WorksStatusHistory { get; set; }
    public ICollection<Language> Languages { get; set; }
    public ICollection<SocietyReference>? SocietyReferences { get; set; }
    public ICollection<ClientReference>? ClientReferences { get; set; }
    public ICollection<Mandate> Mandates { get; set; }
    public ICollection<Document>? Documents { get; set; }
    public ICollection<Registration>? Registrations { get; set; }
    public ICollection<ReRegistration>? ReRegistrations { get; set; }

}