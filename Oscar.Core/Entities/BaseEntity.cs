namespace Oscar.Core.Entities
{
    public abstract class BaseEntity 
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModified { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
