using Oscar.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Oscar.Core.Entities
{
    public class RegistrationBatch : BaseEntity
    {
        public Guid BatchId { get; set; }
        public String? RuntimeParamsJson { get; set; }
        public DateTime DateRegistered { get; set; }
        public String? Notes { get; set; }
        public Enums.RegisterStatus? RegisterStatus { get; set; }
        public int? SocietyId { get; set; }
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsAllClients { get; set; }
        public bool IncludePreviouslyRegistered { get; set; }
        public string? FileName { get; set; }
        public bool? DoNotRegister { get; set; }
    }

}
