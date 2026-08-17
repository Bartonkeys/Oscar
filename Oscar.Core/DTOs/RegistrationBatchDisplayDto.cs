using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record RegistrationBatchDisplayDto
    {
        public List<RegistrationDisplayDto>? registrationDtos;
        public string? RuntimeParamsJson { get; set; }
        public Guid BatchId { get; set; }
        public DateTime DateRegistered { get; set; }
        public string? Notes { get; set; }
        public Enums.RegisterStatus? RegisterStatus { get; set; }
        public int RegistrationCount { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? RegistrationBatchResultPublicUrl { get; set; }
        public string? FileName { get; set; }
        public int? CatalogueId { get; set; }
        public string? CatalogueName { get; set; }
        public int? SocietyId { get; set; }
        public string? Society { get; set; }
        public bool IsAllClients { get; set; }
        public bool? DoNotRegister { get; set; }
    }

}