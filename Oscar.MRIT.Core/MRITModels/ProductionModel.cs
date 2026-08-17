using System;
using System.Collections.Generic;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.MRIT.Core.MRITModels
{
    public record ProductionModel 
    {
        public int Id { get; set; }

        public string EnglishTitle { get; set; }
        public int? Duration { get; set; }
        public DateTime? Date { get; set; }
        public bool IsOneOff { get; set; }

        public string ISAN { get; set; }

        public string IMDBId { get; set; }
        public string RottenTomatoesId { get; set; }
        public string TVDBId { get; set; }
        public string TMDBId { get; set; }
        public string AllMovieId { get; set; }
        public string WikiId { get; set; }
        public string CompactRef { get; set; }

        public IEnumerable<TitleModel> Names { get; set; }

        public IEnumerable<CompanyModel> Companies { get; set; }
        public IEnumerable<CountryModel> Countries { get; set; }
        public IEnumerable<LanguageModel> Languages { get; set; }
        public IEnumerable<PersonModel> People { get; set; }
        public IEnumerable<GenreModel> Genres { get; set; }
        public IEnumerable<EpisodeModel> Episodes { get; set; }
    }

    public record TitleModel
    {
        public string Title { get; set; }
        public string LanguageCode { get; set; }
    }
}
