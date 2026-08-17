using System.ComponentModel.DataAnnotations.Schema;
using Oscar.Core.Common;

namespace Oscar.Core.Entities;

public class ProductionCountryItem
{
    [IgnoreExport]
    public int? WorksId { get; set; }
    [IgnoreExport]
    public int? ClientId { get; set; }
    [IgnoreExport]
    public int? CatalogueId { get; set; }
    [IgnoreExport]
    public int? WorksStatusId { get; set; }

    [Export("Client", true)]
    public string? ClientName { get; set; }
    [Export("Catalogue")]
    public string? CatalogueName { get; set; }
    [Export("Type")]
    public string? Discriminator { get; set; }
    public string? Title { get; set; }
    public string? AlternateTitle { get; set; }
    [Export("Countries Of Production")]
    public string? CountriesOfProduction { get; set; }
    public string? CompactRef { get; set; }
    [IgnoreExport]
    public string? WorksStatus { get; set; }
    [IgnoreExport]
    public string? Released { get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return (string.IsNullOrEmpty(searchText) ||
                (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CatalogueName) && CatalogueName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Discriminator) && Discriminator.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Title) && Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(AlternateTitle) && AlternateTitle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CompactRef) && CompactRef.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CountriesOfProduction) && CountriesOfProduction.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(WorksStatus) && WorksStatus.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(Released) && Released.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (!string.IsNullOrEmpty(CatalogueName) && CatalogueName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (CatalogueId > 0 && CatalogueId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
                (WorksId > 0 && WorksId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) 
            );
    }
}