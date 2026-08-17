using System.ComponentModel.DataAnnotations;

namespace Oscar.Core.Entities
{
    public class WorksImport: BaseEntity
    {
        [StringLength(15)]
        public string? WorksType { get; set; }

        [StringLength(15)]
        public string? SASeriesNumber { get; set; }

        [StringLength(15)]
        public string? SeasonNumber { get; set; }

        [StringLength(15)]
        public string? EpisodeNumber { get; set; }

        [StringLength(5)]
        public string? TitleLanguage { get; set; }

        [StringLength(500)]
        public string? Title { get; set; }

        [StringLength(4)]
        public string? ProductionYear { get; set; }

        [StringLength(5)]
        public string? Duration { get; set; }

        [StringLength(100)]
        public string? DirectorFirstName { get; set; }

        [StringLength(100)]
        public string? DirectorLastName { get; set; }

        [StringLength(200)]
        public string? ProductionCompany1 { get; set; }

        [StringLength(200)]
        public string? ProductionCompany2 { get; set; }

        [StringLength(200)]
        public string? ProductionCompany3 { get; set; }

        [StringLength(100)]
        public string? ProductionCountry1 { get; set; }

        [StringLength(100)]
        public string? ProductionCountry2 { get; set; }

        [StringLength(100)]
        public string? ProductionCountry3 { get; set; }

        [StringLength(100)]
        public string? ProductionCountry4 { get; set; }

        [StringLength(100)]
        public string? Actor1FirstName { get; set; }

        [StringLength(100)]
        public string? Actor1LastName { get; set; }

        [StringLength(100)]
        public string? Actor2FirstName { get; set; }

        [StringLength(100)]
        public string? Actor2LastName { get; set; }

        [StringLength(100)]
        public string? Actor3FirstName { get; set; }

        [StringLength(100)]
        public string? Actor3LastName { get; set; }

        [StringLength(1000)]
        public string? ExcludedCountries { get; set; }

        [StringLength(20)]
        public string? ClaimStartDate { get; set; }

        [StringLength(50)]
        public string? Genre { get; set; }

        [StringLength(50)]
        public string? WorkSubType { get; set; }

        [StringLength(15)]
        public string? CavcoCode { get; set; }

        [StringLength(10)]
        public string? CrtcCode { get; set; }

        [StringLength(500)]
        public string? AKATitle1 { get; set; }

        [StringLength(5)]
        public string? AKATitle1Language { get; set; }

        [StringLength(500)]
        public string? AKATitle2 { get; set; }

        [StringLength(5)]
        public string? AKATitle2Language { get; set; }

        [StringLength(500)]
        public string? AKATitle3 { get; set; }

        [StringLength(5)]
        public string? AKATitle3Language { get; set; }

        [StringLength(15)]
        public string? Colour { get; set; }

        public bool PossibleDuplicate { get; set; }

        public int? WorksId { get; set; }

        public List<WorksRightsImport>? WorksRightsImports { get; set; }

        public WorksImportRequest? WorksImportRequest { get; set; }
    }

}
