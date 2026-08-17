using System.ComponentModel.DataAnnotations.Schema;
using Oscar.Core.Common;

namespace Oscar.Core.Entities;

public class ClientWorkRightItem
{
    [IgnoreExport]
    public int? WorksId { get; set; }
    [IgnoreExport]
    public int? ClientId { get; set; }
    [IgnoreExport]
    public int? CatalogueId { get; set; }
    
    [Export("Client", true)]
    public string? ClientName { get; set; }
    [Export("Catalogue")]
    public string? CatalogueName { get; set; }
    [Export("Type")]
    public string? Discriminator { get; set; }
    public string? Titles { get; set; }
    public string? AlternateTitles { get; set; }
    public int? ProductionYear { get; set; }
    public string? CompactRef { get; set; }
    public string? RightsType { get; set; }
    [Column(TypeName = "datetime2")]
    public DateTime? StartDate { get; set; }
    [Column(TypeName = "datetime2")]
    public DateTime? EndDate { get; set; }
    public decimal? Percentage { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public string? Channel { get; set; }
    public string? Released { get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return (string.IsNullOrEmpty(searchText) ||
                (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Discriminator) && Discriminator.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CompactRef) && CompactRef.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Titles) && Titles.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Released) && Released.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CatalogueName) && CatalogueName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (CatalogueId > 0 && CatalogueId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (WorksId > 0 && WorksId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ProductionYear > 0 && ProductionYear?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
            );
    }
}