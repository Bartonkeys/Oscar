namespace Oscar.Infrastructure.Features.Registration.Services;

public static class RegistrationError
{
    public static string ClientNotFound = "Client not found";
    public static string WorksNotFound = "Work not found";
    public static string CatalogueNotValid = "Catalogue not found or no Client found on Catalog";
    public static string SocietyNotFound = "Society not found";
    public static string InvalidClientStatus = "Client status not valid";
    public static string InvalidClientRights = "Client rights not valid";
    public static string InvalidWorksRights = "Works rights not valid";
    public static string ClientNotLinkedToSociety = "Client not linked to the society";
    public static string InValidWorkStatus = "Work status not valid";
    public static string InValidWorkRightsForSocietyTerritory = "No rights for work in the society territory";
    public static string InValidClientRightsForSocietyTerritory = "No rights for client in the society territory";
    public static string SocietyRightsNotClaimableOnWork = "Rights not claimable by society on work";
    public static string ClientTerminatedBeforeEndOfRegistrationYear = "Client terminated before end of registration year";
    public static string WorksPreviouslyRegisteredBySociety = "Works previously registered by society";
    public static string NoWorks = "No Works selected";
}