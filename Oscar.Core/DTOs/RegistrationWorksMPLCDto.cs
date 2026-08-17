namespace Oscar.Core.DTOs
{
    public class RegistrationWorksMPLCDto
    {
        public string? FileName { get; set; }
        public IEnumerable<MPLCRow> Rows { get; set; } = new List<MPLCRow>();
    }

    public class MPLCRow
    {
        public string? CompactRef { get; set; }
        public string? Title { get; set; }
        public string? WorkType { get; set; }
        public string? ProductionCountry1 { get; set; }
        public string? ProductionCountry2 { get; set; }
        public string? ProductionCountry3 { get; set; }
        public string? OwningClient { get; set; }
        public string? Director1FirstName { get; set; }
        public string? Director1LastName { get; set; }
        public string? Director2FirstName { get; set; }
        public string? Director2LastName { get; set; }
        public string? Director3FirstName { get; set; }
        public string? Director3LastName { get; set; }
        public string? YearOfProduction { get; set; }
    }
}
