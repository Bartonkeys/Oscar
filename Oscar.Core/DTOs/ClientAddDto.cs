using Oscar.Core.Enums;

namespace Oscar.Core.DTOs
{
    public record ClientAddDto
    {
        public int? ClientReference { get; set; }
        public string? ClientName { get; set; }
        public Status? Status { get; set; }
        public AddressAddDto? Address { get; set; }
        public ClientGrade? ClientGrade { get; set; }
        public ClientType? ClientType { get; set; }
        public string? IMaestroClientCode { get; set; }
        public string? IMaestroGroupPayeeCode { get; set; }
        public string? IMaestroGroupPayeeName { get; set; }
        public string? Email { get; set; }
        public string? GeneralNotes { get; set; }
        public ContractDto? Contract { get; set; }
        public ICollection<ClientAltNameDto> ClientAltNames { get; set; }
        public ICollection<ContactDto>? Contacts { get; set; }
        public ICollection<CustomerServiceManagerDto>? CustomServiceManagers { get; set; }
        public string? CCCClientsId { get; set; }
        public string? CRCClientsId { get; set; }
        public string? ScreenRightsPortfolioId { get; set; }
        public string? MPAAClaimantsId { get; set; }
        public string? AgicoaClientRef { get; set; }
    }
}
