using System.Xml;
using System.Xml.Serialization;

namespace Oscar.Core.DTOs
{
    [Serializable]
    [XmlType("Data", Namespace = "")]
    public class RegistrationWorksScreenrightsExport : IRegistrationWorksScreenrights
    {
        [XmlIgnore]
        public string FileName { get; set; }
        public RegistrationWorksScreenrightsExportHeader Header { get; set; }

        [XmlElement]
        public List<RegistrationWorksScreenrightsExportDTO> Work { get; set; }

        public RegistrationWorksScreenrightsExportFooter Footer { get; set; }
    }

    public interface IRegistrationWorksScreenrights
    {
        [XmlIgnore]
        public string FileName { get; set; }
        public RegistrationWorksScreenrightsExportHeader Header { get; set; }
        public List<RegistrationWorksScreenrightsExportDTO> Work { get; set; }
        public RegistrationWorksScreenrightsExportFooter Footer { get; set; }
    }

    [Serializable]
    [XmlType("DataHeader")]
    public class RegistrationWorksScreenrightsExportHeader
    {
        public string Version { get; set; }
        public string FromCompany { get; set; }
        public string FromPerson { get; set; }
        public string ToCompany { get; set; }
        public string ToPerson { get; set; }
        public string BegDate { get; set; }
        public string BegTime { get; set; }
        public string Extensions { get; set; }
    }

    [Serializable]
    [XmlType("DataFooter")]
    public class RegistrationWorksScreenrightsExportFooter
    {
        [XmlElementAttribute(DataType = "integer")]
        public string RecCount { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Extensions { get; set; }
    }

    [Serializable]
    [XmlType("Work")]
    public class RegistrationWorksScreenrightsExportDTO
    {

        [XmlElement(ElementName = "WNS")]
        public string? WorkDeclarationNumberSender { get; set; }

        [XmlElement(ElementName = "WNR")]
        public string? WorkDeclarationNumberReceiver { get; set; }

        //1 = “Serial” 2 = “Season” 3 = “Episode"
        [XmlElement(ElementName = "SL")]
        public int? SerialLevel { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool SerialLevelSpecified { get { return this.SerialLevel != null; } }

        [XmlElement(ElementName = "SNS")]
        public string? SerialNoSender { get; set; }

        [XmlElement(ElementName = "SNR")]
        public string? SerialNoReceiver { get; set; }

        [XmlElement(ElementName = "SnNS")]
        public string? SeasonNoSender { get; set; }

        [XmlElement(ElementName = "SnNR")]
        public string? SeasonNoReceiver { get; set; }

        [XmlElement(ElementName = "SnN")]
        public string? SeasonNo { get; set; }

        [XmlElement(ElementName = "EN")]
        public string? EpisodeNo { get; set; }

        [XmlElement(ElementName = "TSE")]
        public string? TotalSeason { get; set; }

        [XmlElement(ElementName = "TEP")]
        public string? TotalEpisode { get; set; }

        [XmlElement(ElementName = "D")]
        public string? Duration { get; set; }

        //FF = “Feature Film” TF = “Telefilm” SH = “Short Film” SE = “Serial”
        [XmlElement(ElementName = "T")]
        public string? Type { get; set; }

        //FI = “Fiction” NF = “Non Fiction” AN = “Animation”
        [XmlElement(ElementName = "K")]
        public string? Kind { get; set; }

        [XmlElement(ElementName = "YP")]
        public string? YearOfProduction { get; set; }

        [XmlIgnore] //ignoring it as we are not setting any value in this element
        [XmlElement(ElementName = "I")]
        public string? ISAN { get; set; }

        [XmlElement(ElementName = "WD")]
        public string? Withdrawal { get; set; }

        public List<WorksTitleScreenrightsExportDTO>? Ttls { get; set; }

        [XmlArrayItemAttribute("C", IsNullable = false)]
        public string[] CntyPrds { get; set; }

        [XmlArrayItemAttribute("L", IsNullable = false)]
        public string[] Olngs { get; set; }

        public List<WorksInterestedPartiesScreenrightsExportDTO>? IParts { get; set; }
        public List<WorksCompanyOfProductionScreenrightsExportDTO>? CmpyPrds { get; set; }
        public List<WorksRightScreenrightsExportDTO>? Rgts { get; set; }
        public List<WorksMandateScreenrightsExportDTO>? Mandates { get; set; }

    }

    [XmlType("Ttl")]
    public class WorksTitleScreenrightsExportDTO
    {
        [XmlElement(ElementName = "O")]
        public string? Original { get; set; }

        [XmlElement(ElementName = "L")]
        public string? LanguageCode { get; set; }

        [XmlElement(ElementName = "T")]
        public string? Title { get; set; }
    }


    [XmlType("IPart")]
    public class WorksInterestedPartiesScreenrightsExportDTO
    {
        //Possible values: 1 = Director 2 = Actor 3 = Producer 4 = Character 5 = Distributor
        //6 = Script writer 7 = Composer 8 = Screen writer 9 = Anchor 10 = Cinematographer
        [XmlElement(ElementName = "T")]
        public int? Type { get; set; }

        [XmlElement(ElementName = "F")]
        public string? Firstname { get; set; }

        [XmlElement(ElementName = "L")]
        public string? LastName { get; set; }
    }

    [XmlType("CmpyPrd")]
    public class WorksCompanyOfProductionScreenrightsExportDTO
    {
        //Always to "1" for Production company
        [XmlElement(ElementName = "T")]
        public int? CompanyType { get; set; } = 1;

        [XmlElement(ElementName = "N")]
        public string? CompanyName { get; set; }

        //Has anyone ordered the Work Declaration?
        [XmlElement(ElementName = "O")]
        public string? ByOrder { get; set; } = "no";

        //Mandatory when "By order" = yes.
        //If order by = yes, name of the principal who ordered the Work Declaration.
        [XmlElement(ElementName = "P")]
        public string? Principal { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool SerialLevelSpecified { get { return this.Principal != null; } }
    }

    [XmlType("Rgt")]
    public class WorksRightScreenrightsExportDTO
    {
        [XmlElement(ElementName = "RRS")]
        public string? RightsholderReferenceSender { get; set; }

        [XmlElement(ElementName = "RNS")]
        public string? RightsholderNameSender { get; set; } = "Compact Collections";

        [XmlElement(ElementName = "RRR")]
        public int? RightsholderReference { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool RightsholderReferenceSpecified { get { return this.RightsholderReference != null; } }

        [XmlElement(ElementName = "C")]
        public string? CountryOfRetransmission { get; set; }

        [XmlElement(ElementName = "L")]
        public string? LanguageCode { get; set; }

        [XmlElement(ElementName = "Ch")]
        public string? TVChannel { get; set; }

        [XmlElement(ElementName = "P")]
        public float? PercentageOfRight { get; set; }

        [XmlElement(ElementName = "RF")]
        public string RightsFrom { get; set; }

        [XmlElement(ElementName = "RT")]
        public string RightsTo { get; set; }

        [XmlElement(ElementName = "VF")]
        public string ValidityFrom { get; set; }

        [XmlElement(ElementName = "VT")]
        public string ValidityTo { get; set; }

        [XmlElement(ElementName = "T")]
        public string? Rights { get; set; }

        [XmlElement(ElementName = "PFR")]
        public int PercentageOfFilmRights { get; set; }
        [XmlElement(ElementName = "PSR")]
        public int PercentageOfScriptRights { get; set; }
        [XmlElement(ElementName = "PCS")]
        public int PercentageOfCommissionedSoundRights { get; set; }
        [XmlElement(ElementName = "S")]
        public int ServiceElection { get; set; }
    }

    [XmlType("Mandate")]
    public class WorksMandateScreenrightsExportDTO
    {
        [XmlElement(ElementName = "T")]
        public string? MandateType { get; set; }

        [XmlElement(ElementName = "M")]
        public string? Mandated { get; set; }
    }

    public enum ServiceElectionEnum
    {
        AllServicesAustraliaNZAndInternational = 1,
        AllAustralianAndNZServices = 2,
        AllAustralianServices = 3,
        AustralianEducationalCopying = 4,
        AustralianEducationalCommunication = 5,
        AustralianGovernmentCopying = 6,
        AustralianRetransmission = 7,
        NewZealandEducationalCopying = 8, 
        NewZealandEducationalCommunication = 9,
        AustralianAnd_NZServicesExcludingRetransmission = 10,
        InternationalOtherCopying = 11,
        InternationalNon_AU_NZ_PrivateCopying = 12,
        InternationalNon_AU_NZ_PublicLending = 13,
        InternationalNon_AU_NZ_Public_Performance = 14,
        International_Non_AU_NZ_Retransmission = 15
    }

}
