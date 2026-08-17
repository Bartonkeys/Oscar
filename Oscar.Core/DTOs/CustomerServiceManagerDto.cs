namespace Oscar.Core.DTOs
{
    public record CustomerServiceManagerDto : IDto
    {
        public int Id { get; set; }
        public OperatorDto Operator { get; set; }
        public bool IsActive { get; set; }
    }

    public class OperatorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}