using Microsoft.AspNetCore.Http;
using Oscar.Core.Entities;
using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record RegistrationBatchCreateDto
    {
        public List<RegistrationCreateDto>? registrationDtos;
        public String? RuntimeParamsJson { get; set; }
        public Guid BatchId { get; set; }
        public bool IsAllClients { get; set; }
    }

}