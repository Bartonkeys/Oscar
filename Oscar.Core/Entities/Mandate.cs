namespace Oscar.Core.Entities
{
    public class Mandate : BaseEntity
    {
        public MandateType MandateType { get; set; }
        public bool Mandated { get; set; }

        public Client? Client { get; set; }
        public Catalogue? Catalogue { get; set; }
        public Works? Works { get; set; }
    }

    public class MandateType : LookUpEntity
    {
        public ICollection<Mandate>? Mandates { get; set; }
    }
}
