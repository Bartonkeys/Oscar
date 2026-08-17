namespace Oscar.Core.Entities;

public class WorksStatusHistory: BaseEntity
{
    public WorksStatus OldStatus { get; set; }
    public WorksStatus NewStatus { get; set; }
    public DateTime DateChanged { get; set; }
    public string Username { get; set; }
}