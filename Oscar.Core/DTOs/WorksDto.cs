using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{

    public record WorksDto
    {
        public int Id { get; set; }
        public Enums.WorksStatus? WorksStatus { get; set; }
        public Enums.CommissionedWorkStatus? CommissionedWorkStatus { get; set; }
        public string? UncontrolledReason { get; set; }
        public int? GenreId { get; set; }
        public GenreDto? Genre { get; set; }
        public WorksTypeDto? WorksType { get; set; }
        public WorksSubTypeDto? WorksSubType { get; set; }
        public int? DurationMinutes { get; set; }
        public int? ProductionYear { get; set; }
        public int? FirstBroadcastYear { get; set; }
        public string? IMaestroWorkCode { get; set; }
        public string? AgicoaWorksReference { get; set; }
        public string? Isan { get; set; }
        public string? CavcoCode { get; set; }
        public string? CrtcCode { get; set; }
        public string? GeneralNotes { get; set; }
        public int? Number { get; set; }
        public string? ColourFormat { get; set; }
        public int? WorksTypeId { get; set; }
        public int? WorksSubTypeId { get; set; }
        public int? GenreSubTypeId { get; set; }
        public string? CompactRef { get; set; }
        public string? AS400RefNo { get; set; }
        public string? Nationality { get; set; }

        public string? Discriminator { get; set; }

        public bool Selected { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<ClientDto>? Clients { get; set; }
        public ICollection<CatalogueDto>? Catalogues { get; set; }

        public int DisplayTitleId { get; set; }
        public string? DisplayTitle { get; set; }
        public TitleType DisplayTitleType { get; set; } = TitleType.Main;
        public ICollection<WorksTitleDto>? Titles { get; set; }
        public ICollection<ConflictDto>? Conflicts { get; set; }
        public ICollection<WorksTypeDto>? WorksSubTypes { get; set; }
        public ICollection<CountryDto>? Countries { get; set; }
        public ICollection<CompanyDto>? Companies { get; set; }
        public ICollection<AlternativeTitleDto>? AlternativeTitles { get; set; }
        public ICollection<PersonDto>? Producers { get; set; }
        public ICollection<PersonDto> Directors { get; set; }
        public ICollection<PersonDto>? Actors { get; set; }
        public ICollection<PersonDto>? Distributors { get; set; }
        public ICollection<PersonDto>? ScreenWriters { get; set; }
        public ICollection<PersonDto>? ScriptWriters { get; set; }
        public ICollection<WorksStatusHistoryDto> WorksStatusHistory { get; set; }
        public ICollection<SocietyReferenceDto>? SocietyReferences { get; set; }
        public ICollection<ClientReferenceDto>? ClientReferences { get; set; }
        public ICollection<DocumentDto>? Documents { get; set; }
        public ICollection<LanguageDto>? Languages { get; set; }
        public ICollection<RegistrationDisplayDto>? Registrations { get; set; }
        public ICollection<ReRegistrationDto>? ReRegistrations { get; set; }
        public ICollection<MandateDto> Mandates { get; set; }
    }

    public class ClientReferenceDto
    {
        public int RHId;
        public int ClientId { get; set; }
        public string ClientName {get; set; }
        public string? AgicoaDeclarationNumber { get; set; }
    }

    public class SocietyReferenceDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public SocietyDto Society { get; set; }
        public int SocietyId { get; set; }
        public string SocietyName { get; set; }
        public int WorksId { get; set; }
    }



    public sealed class WorksDtoMap : CsvHelper.Configuration.ClassMap<WorksDto>
    {
        public WorksDtoMap()
        {
            Map(m => m.Id);
            Map(m => m.Discriminator).Optional();
            Map(m => m.Titles).Name("Titles").Convert(args =>
            {
                return string.Join("; ", args.Value.Titles.Select(d => d.Title));
            });
            Map(m => m.CompactRef).Optional();
            Map(m => m.ProductionYear).Optional();
            Map(m => m.FirstBroadcastYear).Optional();
            Map(m => m.WorksStatus).Optional();
            Map(m => m.Clients).Name("Client").Convert(args =>
            {
                return string.Join("; ", args.Value.Clients.Select(d => d.ClientName));
            }); Map(m => m.Catalogues).Name("Catalogue").Convert(args =>
            {
                return string.Join("; ", args.Value.Catalogues.Select(d => d.Name));
            });

        }

        public sealed class TitleMap : CsvHelper.Configuration.ClassMap<WorksTitleDto>
        {
            public TitleMap()
            {
                Map(m => m.Title != null ? m.Title : "n/a");    
            }
        }
        public sealed class ClientMap : CsvHelper.Configuration.ClassMap<ClientBasicDto>
        {
            public ClientMap()
            {
                Map(m => m.ClientName != null ? m.ClientName : "n/a");
            }
        }
        public sealed class CatalogueMap : CsvHelper.Configuration.ClassMap<CatalogueDto>
        {
            public CatalogueMap()
            {
                Map(m => m.Name != null ? m.Name : "n/a");
            }
        }
    }
}
