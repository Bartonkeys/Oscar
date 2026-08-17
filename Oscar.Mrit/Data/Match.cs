using System.Collections.Generic;

namespace Oscar.Mrit.Data
{
    public class Match: BaseEntity
    {
        public int ProductionId { get; set; }
        public int RecordId { get; set; }
        public string ProductionTitle { get; set; }
        public string RecordTitle { get; set; }
        public int? ProductionYear { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public BatchJob BatchJob { get; set; }
        public ICollection<AltProductionTitle> AltProductionTitles { get; set; }
        public ICollection<AltRecordTitle> AltRecordTitles { get; set; }
        public ICollection<Transmission> Transmissions { get; set; }
        public ICollection<Works> Works { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Country> Countries { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Language> Languages { get; set; }
        public ICollection<PersonOfInterest> PersonOfInterests { get; set; }
    }
}
