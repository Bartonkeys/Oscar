namespace Oscar.Core.Entities;

public class Operator : BaseEntity
{
    public string FullName { get; set; }
    public ICollection<CustomerServiceManager> CustomerServiceManagers { get; set; }
}