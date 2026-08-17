namespace Oscar.MRIT.Core.MRITModels
{
    public record CompanyModel
    {
        public string MainName { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
