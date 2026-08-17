namespace Oscar.Core.DTOs
{
    public record ContractDto
    {
        public int Id { get; set; }
        public DateTime? FirstStartDate { get; set; }
        public DateTime? CurrentStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public bool? Terminated { get; set; } = false;
        public string? Notes { get; set; }
        public string? ParentCompany { get; set; }
        public string? AccountingCurrency { get; set; }
        public string? Email { get; set; }
    }
}