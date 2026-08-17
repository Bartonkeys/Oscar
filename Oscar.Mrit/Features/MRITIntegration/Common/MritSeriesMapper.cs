using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.Mrit.Features.MRITIntegration.Common
{
    public class MritSeriesMapper: MritMapper
    {
        private List<VwOnMusicFelixWorks> _felixEpisodes;

        public MritSeriesMapper(OscarContext oscarContext, VwOnMusicFelixWorks felixWork) : base(oscarContext, felixWork)
        {

        }

        protected override async Task Process()
        {
            ProcessAtProductionLevel();
            ProcessAtEpisodeLevel();
        }

        private void ProcessAtProductionLevel()
        {
            MritProductionModel.People = FelixWork.GetPeopleFrom();

            var felixSeasonHeaders = OscarCOntext.VwOnMusicFelixWorks
                .Where(f => f.SerialLevel == (int)SerialLevel.SeasonHeader && f.SeriesRef == FelixWork.CompactRef)
                .ToList();

            var peopleList = MritProductionModel.People.ToList();
            foreach (var seasonHeader in felixSeasonHeaders)
                peopleList.AddRange(seasonHeader.GetPeopleFrom());

            MritProductionModel.People = peopleList.Distinct().AsEnumerable();
        }

        private void ProcessAtEpisodeLevel()
        {
            _felixEpisodes = OscarCOntext.VwOnMusicFelixWorks
                .Where(f => f.SerialLevel == (int)SerialLevel.Episode && f.SeriesRef == FelixWork.CompactRef)
                .ToList();

            if (_felixEpisodes.Any()) ProcessEpisodes();
        }

        private void ProcessEpisodes()
        {
            var mritEpisodes = new List<EpisodeModel>();
            foreach (var felixEpisode in _felixEpisodes)
            {
                var episodeTitles = felixEpisode.Titles != null ? JsonConvert.DeserializeObject<IEnumerable<TitleModel>>(felixEpisode.Titles) 
                    : new List<TitleModel>()
                    {
                        new() {Title = "No title", LanguageCode = "ENG"}
                    };

                foreach (var titleModel in episodeTitles)
                {
                    titleModel.Title = titleModel.Title.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title);
                }

                var mritEpisode = new EpisodeModel
                {
                    Title = episodeTitles.First().Title,
                    Duration = felixEpisode.Duration,
                    EpisodeNumber = felixEpisode.EpisodeRef,
                    SeasonNumber = felixEpisode.SeasonNo,
                    Titles = episodeTitles,
                    Date = felixEpisode.ProductionYear == null || !Regex.IsMatch(felixEpisode.ProductionYear.ToString(), DatePattern)
                        ? null : new DateTime(felixEpisode.ProductionYear.Value, 1, 1)
                };

                mritEpisode.People = felixEpisode.GetPeopleFrom();
                mritEpisodes.Add(mritEpisode);
            }

            MritProductionModel.Episodes = mritEpisodes;
        }

    }
}
