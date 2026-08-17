using System;
using System.Collections.Generic;

namespace Oscar.MRIT.Core.MRITModels
{
    public record EpisodeModel
    {
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public int? Duration { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }

        public IEnumerable<TitleModel> Titles { get; set; }
        public IEnumerable<PersonModel> People { get; set; }
    }
}
