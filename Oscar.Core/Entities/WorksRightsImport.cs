namespace Oscar.Core.Entities;

public class WorksRightsImport: BaseEntity
{
    public WorksImport? WorksImport { get; set; }
    public string TypeName { get; set; }
    public string CountryCode { get; set; }
    public string LanguageName { get; set; }
    public string ChannelName { get; set; }
    public DateTime StartOfRight { get; set; }
    public DateTime EndOfRight { get; set; }
    public DateTime StartOfValidity { get; set; }
    public DateTime EndOfValidity { get; set; }
    public decimal? Percentage { get; set; }
}