namespace Oscar.Core.Entities
{
    public class Client: BaseEntity
    {
        private string? _clientName;
        public string? ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
            }
        }

        public string? IMaestroClientCode { get; set; }
        public string? IMaestroGroupPayeeCode { get; set; }
        public string? IMaestroGroupPayeeName { get; set; }
        public string? Email { get; set; }
        public string? GeneralNotes { get; set; }
        public Enums.Status? Status { get; set; }
        public Enums.ClientGrade? ClientGrade { get; set; }
        public Enums.ClientType? ClientType { get; set; }
        public int? ContractId { get; set; }
        public Contract? Contract { get; set; }

        public int? ClientReference { get; set; }
        public string? CCCClientsId { get; set; }
        public string? CRCClientsId { get; set; }
        public string? ScreenRightsPortfolioId { get; set; }
        public string? MPAAClaimantsId { get; set; }
        public string? AgicoaClientRef { get; set; }

        public ICollection<Address>? Addresses { get; set; }
        public ICollection<CustomerServiceManager>? CustomerServiceManagers { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
        public ICollection<OtherName>? OtherNames { get; set; }
        public ICollection<Right>? Rights { get; set; }
        public ICollection<SocietyReference>? SocietyReferences { get; set; }
        public ICollection<Society>? Societies { get; set; }
        public ICollection<Catalogue> Catalogues { get; set; }
        public ICollection<Works> Works { get; set; }
        public ICollection<WorksImportRequest> WorksImportRequests { get; set; }
        public ICollection<Mandate> Mandates { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<ClientAltName> ClientAltNames { get; set; }

    }
}
