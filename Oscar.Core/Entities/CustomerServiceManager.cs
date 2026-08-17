namespace Oscar.Core.Entities;

public class CustomerServiceManager: BaseEntity
{
    public bool IsActive { get; set; }

    public Operator Operator { get; set; }
    public Client Client { get; set; }
}