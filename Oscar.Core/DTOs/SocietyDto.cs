namespace Oscar.Core.DTOs
{
    public record SocietyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExpandedName { get; set; }
        public string GeneralNotes { get; set; }
        public bool? IsClientRegistration { get; set; }
        public bool? IsWorksRegistration { get; set; }
        public string? ClientRegistrationNotes { get; set; }
        public string? WorksRegistrationNotes { get; set; }
        public string? Website { get; set; }

        public ICollection<AddressDto>? Addresses { get; set; }
        public ICollection<ContactDto> Contacts { get; set; }
        public ICollection<ClientDto> Clients { get; set; }
        public ICollection<SocietyRightsDto> SocietyRights { get; set; }
    }

    public class SocietyRightsDto: IDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public RightsTypeDto RightsType { get; set; }
        public CountryDto Country { get; set; }
    }

}
