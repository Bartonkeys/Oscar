namespace Oscar.MRIT.Core.Configuration
{
    public record BatchSettings
    {
        public int Size { get; set; }
        public bool UseFelixAPI { get; set; }
    }
}
