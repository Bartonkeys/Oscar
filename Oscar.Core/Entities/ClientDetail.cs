
using Oscar.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oscar.Core.Entities;

public class ClientDetail
{
    [IgnoreExport]
    public int? ClientId { get; set; }

    [Export("Client")]
    public string? ClientName { get; set; }
    [Export("Status")]
    public string? ClientStatus { get; set; }

    public int? ClientReference { get; set; }

    public int? ClientType { get; set; }
    [Export("Grade")]
    public string? ClientGrade { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime ClientCreatedOn { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime ClientStartOn { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime ClientEndOn { get; set; }

    public string? ClientEmail { get; set; }

    public string? AgicoaClientRef { get; set; }

    public string? CRCClientsId { get; set; }

    public string? CCCClientsId { get; set; }

    public string? MPAAClaimantsId { get; set; }

    public string? ScreenRightsPortfolioId { get; set; }

    public string? AccountingCurrency { get; set; }

    public string? AutoRenew { get; set; }

    public string? ParentCompany { get; set; }

    public string? ContractEmail { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContractFirstStartDate { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContractCurrentStartDate { get; set; }

    public string? ContractTerminated { get; set; }

    public DateTime? ContractEndDate { get; set; }

    //public string? ContractNotes { get; set; }

    public string? As400RefNum { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public string? AddressLine4 { get; set; }

    public string? Country { get; set; }

    public string? AddressEmail { get; set; }

    public string? PostZipCode { get; set; }

    public string? Website { get; set; }

    public string? Title { get; set; }

    public string? ContactFirstName { get; set; }

    public string? ContactLastName { get; set; }

    public string? ContactEmail { get; set; }

    public string? JobTitle { get; set; }

    public string? ContactMobile { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContactStartDate { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContactEndDate { get; set; }

    public string? ContactType { get; set; }

    //public string? ContactComments { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime? ContactCreationDate { get; set; }

    public string? ContactWebsite { get; set; }
    [Export("CSM")]
    public string? AccountManager { get; set; }

    public string? IMaestroGroupPayeeCode { get; set; }
    public string? IMaestroGroupPayeeName { get; set; }

    public virtual bool FilterBy(string searchText)
    {
        return ( string.IsNullOrEmpty(searchText) ||
               (!string.IsNullOrEmpty(ClientName) && ClientName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ContactFirstName) && ContactFirstName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ContactLastName) && ContactLastName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ContactEmail) && ContactEmail.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(Title) && Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(JobTitle) && JobTitle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(Website) && Website.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(Country) && Country.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(As400RefNum) && As400RefNum.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ClientGrade) && ClientGrade.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ClientStatus) && ClientStatus.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ContractTerminated) && ContractTerminated.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ClientEmail) && ClientEmail.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(AddressEmail) && AddressEmail.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(AddressLine1) && AddressLine1.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(AddressLine2) && AddressLine2.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(AddressLine3) && AddressLine3.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(PostZipCode) && PostZipCode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(ParentCompany) && ParentCompany.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (!string.IsNullOrEmpty(AccountManager) && AccountManager.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (ClientId > 0 && ClientId?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1) ||
               (ClientReference > 0 && ClientReference?.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1)
            );
    }
}