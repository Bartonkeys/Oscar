using Oscar.Core.Enums;

namespace Oscar.Core.Entities
{
    public class Registration : BaseEntity
    {
        public RegistrationBatch? RegistrationBatch { get; set; }
        public Client? Client { get; set; }
        public Catalogue? Catalogue { get; set; }
        public Works? Works { get; set; }
        public Society? Society { get; set; }
        public DateTime? DateRegistered { get; set; }
        public Enums.RegisterType? RegisterType { get; set; }
        public Enums.RegisterStatus? RegisterStatus { get; set; }
        public string? Notes { get; set;}

    }
}
