using System.Xml;
using System.Xml.Serialization;

namespace Oscar.Core.DTOs
{
    [Serializable]
    [XmlType("Data", Namespace = "")]
    public class RegistrationWorksAgicoaExport: IRegistration
    {
        [XmlIgnore]
        public string FileName { get; set; }
        public RegistrationWorksAgicoaExportHeader Header { get; set; }

        [XmlElement]
        public List<RegistrationWorksAgicoaExportDTO> Work { get; set; }

        public RegistrationWorksAgicoaExportFooter Footer { get; set; }
    }

    [Serializable]
    [XmlType("Data", Namespace = "")]
    public class RegistrationWorksSuissImageExport : IRegistration
    {
        [XmlIgnore]
        public string FileName { get; set; }

        public RegistrationWorksAgicoaExportHeader Header { get; set; }

        [XmlElement]
        public List<RegistrationWorksAgicoaExportDTO> Work { get; set; }

        public RegistrationWorksAgicoaExportFooter Footer { get; set; }
    }

    public interface IRegistration
    {
        [XmlIgnore]
        public string FileName { get; set; }
        public RegistrationWorksAgicoaExportHeader Header { get; set; }
        public List<RegistrationWorksAgicoaExportDTO> Work { get; set; }
        public RegistrationWorksAgicoaExportFooter Footer { get; set; }
    }

    [Serializable]
    [XmlType("DataHeader")]
    public class RegistrationWorksAgicoaExportHeader
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
    public class RegistrationWorksAgicoaExportFooter
    {
        [XmlElementAttribute(DataType = "integer")]
        public string RecCount { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Extensions { get; set; }
    }

    [Serializable]
    [XmlType("Work")]
    public class RegistrationWorksAgicoaExportDTO
    {
        [XmlIgnore]
        public bool IsRejected { get; set; } = false;

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

        [XmlElement(ElementName = "En")]
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

        [XmlElement(ElementName = "I")]
        public string? ISAN { get; set; }

        [XmlElement(ElementName = "WD")]
        public string? Withdrawal { get; set; }

        public List<WorksTitleAgicoaExportDTO>? Ttls { get; set; }

        [XmlArrayItemAttribute("C", IsNullable = false)]
        public string[] CntyPrds { get; set; }

        [XmlArrayItemAttribute("L", IsNullable = false)]
        public string[] Olngs { get; set; }

        public List<WorksInterestedPartiesAgicoaExportDTO>? IParts { get; set; }
        public List<WorksCompanyOfProductionAgicoaExportDTO>? CmpyPrds { get; set; }
        public List<WorksRightAgicoaExportDTO>? Rgts { get; set; }
        public List<WorksMandateAgicoaExportDTO>? Mandates { get; set; }

    }

    [XmlType("Ttl")]
    public class WorksTitleAgicoaExportDTO
    {
        [XmlElement(ElementName = "O")]
        public string? Original { get; set; }

        [XmlElement(ElementName = "L")]
        public string? LanguageCode { get; set; }

        [XmlElement(ElementName = "T")]
        public string? Title { get; set; }
    }


    [XmlType("IPart")]
    public class WorksInterestedPartiesAgicoaExportDTO
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
    public class WorksCompanyOfProductionAgicoaExportDTO
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
    public class WorksRightAgicoaExportDTO
    {
        [XmlElement(ElementName = "RRS")]
        public string? RightsholderReferenceSender { get; set; }

        [XmlElement(ElementName = "RNS")]
        public string? RightsholderNameSender { get; set; } = "COMPACT COLLECTIONS";

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

        [XmlElement(ElementName = "RF", DataType = "date")]
        public DateTime? RightsFrom { get; set; }

        [XmlElement(ElementName = "RT", DataType = "date")]
        public DateTime? RightsTo { get; set; }

        [XmlElement(ElementName = "VF", DataType = "date")]
        public DateTime? ValidityFrom { get; set; }

        [XmlElement(ElementName = "VT", DataType = "date")]
        public DateTime? ValidityTo { get; set; }

        [XmlElement(ElementName = "T")]
        public string? Rights { get; set; }

    }

    [XmlType("Mandate")]
    public class WorksMandateAgicoaExportDTO
    {
        [XmlElement(ElementName = "T")]
        public string? MandateType { get; set; }

        [XmlElement(ElementName = "M")]
        public string? Mandated { get; set; }
    }

}

