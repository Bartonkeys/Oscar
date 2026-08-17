using System.Globalization;
using System.Xml.Serialization;

namespace Oscar.Core.DTOs
{
    [XmlRoot("Data")]
    public class WorksImportAgicoaDto
    {
        [XmlElement("Header")]
        public Header Header { get; set; }

        [XmlElement("Work")]
        public List<Work> Works { get; set; }

        [XmlElement("Footer")]
        public Footer Footer { get; set; }
    }

    public class Header
    {
        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("FromCompany")]
        public string FromCompany { get; set; }

        [XmlElement("FromPerson")]
        public string FromPerson { get; set; }

        [XmlElement("ToCompany")]
        public string ToCompany { get; set; }

        [XmlElement("ToPerson")]
        public string ToPerson { get; set; }

        [XmlElement("BegDate")]
        public string BegDate { get; set; }

        [XmlElement("BegTime")]
        public string BegTime { get; set; }

        [XmlElement("Extensions")]
        public string Extensions { get; set; }
    }

    public class Work
    {
        [XmlElement("WNS")]
        public string WNS { get; set; }

        [XmlElement("WNR")]
        public string WNR { get; set; }

        [XmlElement("SL")]
        public string SL { get; set; }

        [XmlElement("SNS")]
        public string SNS { get; set; }

        [XmlElement("SNR")]
        public string SNR { get; set; }

        [XmlElement("SnNS")]
        public string SnNS { get; set; }

        [XmlElement("SnNR")]
        public string SnNR { get; set; }

        [XmlElement("SnN")]
        public string SnN { get; set; }

        [XmlElement("EN")]
        public string EN { get; set; }

        [XmlElement("TSE")]
        public int TSE { get; set; }

        [XmlElement("TEP")]
        public int TEP { get; set; }

        [XmlElement("D")]
        public int Duration { get; set; }

        [XmlElement("T")]
        public string WorksType { get; set; }

        [XmlElement("K")]
        public string Genre { get; set; }

        [XmlElement("YP")]
        public int YP { get; set; }

        [XmlElement("I")]
        public string I { get; set; }

        [XmlElement("WD")]
        public string WD { get; set; }

        [XmlArray("Ttls")]
        [XmlArrayItem("Ttl")]
        public List<Title> Titles { get; set; }

        [XmlArray("CntyPrds")]
        [XmlArrayItem("C")]
        public List<string> CountryProductions { get; set; }

        [XmlArray("Olngs")]
        [XmlArrayItem("L")]
        public List<string> OriginalLanguages { get; set; }

        [XmlArray("IParts")]
        [XmlArrayItem("IPart")]
        public List<IPart> Parts { get; set; }

        [XmlArray("CmpyPrds")]
        [XmlArrayItem("CmpyPrd")]
        public List<CompanyProduction> CompanyProductions { get; set; }

        [XmlArray("Rgts")]
        [XmlArrayItem("Rgt")]
        public List<AgicoaRight> Rights { get; set; }

        [XmlArray("Mandates")]
        [XmlArrayItem("Mandate")]
        public List<AgicoaMandate> Mandates { get; set; }
    }

    public class Title
    {
        [XmlElement("O")]
        public string Original { get; set; }

        [XmlElement("L")]
        public string Language { get; set; }

        [XmlElement("T")]
        public string Text { get; set; }
    }

    public class IPart
    {
        [XmlElement("T")]
        public string Type { get; set; }

        [XmlElement("L")]
        public string LastName { get; set; }

        [XmlElement("F")]
        public string FirstName { get; set; }
    }

    public class CompanyProduction
    {
        [XmlElement("T")]
        public string Type { get; set; }

        [XmlElement("N")]
        public string Name { get; set; }

        [XmlElement("O")]
        public string Other { get; set; }

        [XmlElement("P")]
        public string P { get; set; }
    }

    public class AgicoaRight
    {
        [XmlElement("RRS")]
        public string RRS { get; set; }

        [XmlElement("RNS")]
        public string RNS { get; set; }

        [XmlElement("RRR")]
        public string RRR { get; set; }

        [XmlElement("C")]
        public string CountryCode { get; set; }

        [XmlElement("L")]
        public string LanguageName { get; set; }

        [XmlElement("Ch")]
        public string ChannelName { get; set; }

        [XmlElement("P")]
        public int Percentage { get; set; }

        [XmlElement("RF")]
        public string StartOfRight { get; set; }

        [XmlElement("RT")]
        public string EndOfRight { get; set; }

        [XmlElement("VF")]
        public string StartOfValidity { get; set; }

        [XmlElement("VT")]
        public string EndOfValidity { get; set; }

        [XmlElement("T")]
        public string Type { get; set; }
    }

    public class AgicoaMandate
    {
        [XmlElement("T")]
        public string Type { get; set; }

        [XmlElement("M")]
        public string M { get; set; }
    }

    public class Footer
    {
        [XmlElement("RecCount")]
        public int RecCount { get; set; }

        [XmlElement("EndDate")]
        public string EndDate { get; set; }

        [XmlElement("EndTime")]
        public string EndTime { get; set; }

        [XmlElement("Extensions")]
        public string Extensions { get; set; }
    }

    public static class WorksImportMapper
    {
        public static List<WorksImportDto> Map(WorksImportAgicoaDto source)
        {
            var worksImportDtos = new List<WorksImportDto>();

            foreach (var work in source.Works)
            {
                var worksImportDto = new WorksImportDto
                {
                    WorksType = (work.SL == "1") ? "Series" : (work.SL == "2") ? "Season" : (work.SL == "3") ? "Episode" : "Stand Alone",

                    SASeriesNumber = (work.SL == "1" || string.IsNullOrEmpty(work.SL)) ? work.WNS : work.SNS,
                    SeasonNumber = (work.SL == "2") ? work.WNS : work.SnNS,
                    EpisodeNumber = (work.SL == "3") ? work.WNS : null,

                    Title = work.Titles?.FirstOrDefault(t => t.Original.ToLower() is "yes" or "y")?.Text,
                    TitleLanguage = work.Titles?.FirstOrDefault(t => t.Original.ToLower() is "yes" or "y")?.Language, //todo add this in WorksImport entity

                    AKATitle1 = work.Titles?.FirstOrDefault(t => t.Original.ToLower() is "no" or "n")?.Text,
                    AKATitle1Language = work.Titles?.FirstOrDefault(t => t.Original.ToLower() is "no" or "n")?.Language,

                    ProductionYear = work.YP.ToString(),
                    Duration = work.Duration.ToString(),
                    DirectorFirstName = work.Parts?.FirstOrDefault(p => p.Type == "Director")?.FirstName,
                    DirectorLastName = work.Parts?.FirstOrDefault(p => p.Type == "Director")?.LastName,
                    ProductionCompany1 = work.CompanyProductions?.ElementAtOrDefault(0)?.Name,
                    ProductionCompany2 = work.CompanyProductions?.ElementAtOrDefault(1)?.Name,
                    ProductionCompany3 = work.CompanyProductions?.ElementAtOrDefault(2)?.Name,
                    ProductionCountry1 = work.CountryProductions?.ElementAtOrDefault(0),
                    ProductionCountry2 = work.CountryProductions?.ElementAtOrDefault(1),
                    ProductionCountry3 = work.CountryProductions?.ElementAtOrDefault(2),
                    ProductionCountry4 = work.CountryProductions?.ElementAtOrDefault(3),
                    Actor1FirstName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(0)?.FirstName,
                    Actor1LastName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(0)?.LastName,
                    Actor2FirstName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(1)?.FirstName,
                    Actor2LastName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(1)?.LastName,
                    Actor3FirstName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(2)?.FirstName,
                    Actor3LastName = work.Parts?.Where(p => p.Type == "Actor").ElementAtOrDefault(2)?.LastName,
                    ExcludedCountries = string.Join(",", work?.Rights?.Where(r => r.Percentage == 0)?.Select(r => r.CountryCode)),
                    Genre = work.Genre,
                    WorkSubType = work.SnNR,

                    AKATitle2 = work.Titles?.Where(t => t.Original == "no").ElementAtOrDefault(1)?.Text,
                    AKATitle2Language = work.Titles?.Where(t => t.Original == "no").ElementAtOrDefault(1)?.Language,
                    AKATitle3 = work.Titles?.Where(t => t.Original == "no").ElementAtOrDefault(2)?.Text,
                    AKATitle3Language = work.Titles?.Where(t => t.Original == "no").ElementAtOrDefault(2)?.Language
                };

                if (work.Rights?.Count > 0)
                {
                    worksImportDto.WorksRightsImports = new List<WorksRightsImportDto>();

                    foreach (var right in work.Rights)
                    {
                        worksImportDto.WorksRightsImports.Add
                        (
                            new WorksRightsImportDto 
                            {
                                TypeName = right.Type,
                                CountryCode = right.CountryCode,
                                LanguageName = right.LanguageName,
                                ChannelName = right.ChannelName,
                                Percentage = right.Percentage,
                                StartOfRight = DateTime.ParseExact(right.StartOfRight, "yyyy/MM/dd", CultureInfo.InvariantCulture), 
                                EndOfRight = DateTime.ParseExact(right.EndOfRight, "yyyy/MM/dd", CultureInfo.InvariantCulture),
                                StartOfValidity = DateTime.ParseExact(right.StartOfValidity, "yyyy/MM/dd", CultureInfo.InvariantCulture),
                                EndOfValidity = DateTime.ParseExact(right.EndOfValidity, "yyyy/MM/dd", CultureInfo.InvariantCulture),
                            }
                        );
                    }
                }
                worksImportDtos.Add(worksImportDto);
            }

            return worksImportDtos;
        }
    }
}
