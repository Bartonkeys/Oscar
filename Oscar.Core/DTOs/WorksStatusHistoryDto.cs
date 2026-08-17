namespace Oscar.Core.DTOs
{
    public class WorksStatusHistoryDto
    {
        public int Id { get; set; }
        public WorksStatusDto OldStatus { get; set; }
        public WorksStatusDto NewStatus { get; set; }
        public DateTime DateChanged { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}

