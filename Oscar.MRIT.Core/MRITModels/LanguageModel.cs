
namespace Oscar.MRIT.Core.MRITModels
{
    public record LanguageModel
    {
        public string MainName { get; set; }
        public string ISO639_2 { get; set; }
        public string ISO639_1 { get; set; }
    }
}
