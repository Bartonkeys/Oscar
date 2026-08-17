using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record RegistrationCreateDto
    {
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }
        public int? WorksId { get; set; }
        public int? SocietyId { get; set; }
        public DateTime? DateRegistered { get; set; }
        public Enums.RegisterType? RegisterType { get; set; }
        public Enums.RegisterStatus? RegisterStatus { get; set; }
        public string? Notes { get; set; }

        //Parameterless constructor required for xml serializer
        public RegistrationCreateDto() { }

    }

    public sealed class RegistrationCreateMap : CsvHelper.Configuration.ClassMap<RegistrationCreateDto>
    {
        public RegistrationCreateMap()
        {
            Map(m => m.ClientId);
            Map(m => m.CatalogueId).Optional();
            Map(m => m.WorksId).Optional();
            Map(m => m.SocietyId).Optional();
            Map(m => m.DateRegistered).Optional();
            Map(m => m.RegisterStatus).Optional();
            Map(m => m.RegisterType).Optional();
            Map(m => m.Notes).Optional();
        }
    }
}