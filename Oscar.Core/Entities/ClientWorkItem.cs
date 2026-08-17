using System.ComponentModel.DataAnnotations.Schema;
using Oscar.Core.Common;
using Oscar.Core.Enums;

namespace Oscar.Core.Entities;

public class ClientWorkItem
{
    [IgnoreExport] 
    public int? WorksId { get; set; }
    [IgnoreExport] 
    public int? ClientId { get; set; }
    [IgnoreExport] 
    public int? CatalogueId { get; set; }
    [IgnoreExport] public int? WorksStatusId { get; set; }
    [Export("Client")]
    public string? ClientName { get; set; }
    [Export("Catalogue")]
    public string? CatalogueName { get; set; }
    [Export("Type")]
    public string? Discriminator { get; set; }
    [Export("Title")]
    public string? Titles { get; set; }
    [Export("Alt Title")]
    public string? AlternateTitles { get; set; }
    [Export("Season Name")]
    public string? SeasonTitle { get; set; }
    [Export("Series Name")]
    public string? SeriesTitle { get; set; }
    [Export("Production Countries")]
    public string? CountriesOfProduction { get; set; }
    [Export("Companies")]
    public string? Companies { get; set; }
    [Export("Production Year")]
    public int? ProductionYear { get; set; }
    [Export("Agicoa Reference")]
    public string? AgicoaWorksReference { get; set; }
    [Export("Declaration No")]
    public string? AgicoaDeclarationNumber { get; set; }
    [Export("Compact Reference")]
    public string? CompactRef { get; set; }
    [Export("AS400 RefNo")] 
    public string? AS400RefNo { get; set; }
    [Export("Status")]
    public string? WorksStatus { get; set; }
    [IgnoreExport]
    public string? Released { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? CreationDate { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? LastModified { get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return (string.IsNullOrEmpty(searchText) ||
                (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Discriminator) && Discriminator.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AgicoaWorksReference) && AgicoaWorksReference.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AgicoaDeclarationNumber) && AgicoaDeclarationNumber.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CompactRef) && CompactRef.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AS400RefNo) && AS400RefNo.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Titles) && Titles.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AlternateTitles) && AlternateTitles.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(SeriesTitle) && SeriesTitle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(SeasonTitle) && SeasonTitle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(WorksStatus) && WorksStatus.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Released) && Released.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CatalogueName) && CatalogueName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CountriesOfProduction) && CountriesOfProduction.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Companies) && Companies.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (CatalogueId > 0 && CatalogueId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (WorksId > 0 && WorksId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ProductionYear > 0 && ProductionYear?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
            );
    }
}