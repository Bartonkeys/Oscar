using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.Mrit.Features.MRITIntegration.Common
{
    public interface IMritMapper
    {
        Task<ProductionModel> MapFrom();
    }

    public abstract class MritMapper : IMritMapper
    {
        protected readonly OscarContext OscarCOntext;
        protected readonly VwOnMusicFelixWorks FelixWork;
        protected readonly ProductionModel MritProductionModel;
        protected string DatePattern = @"^\d{4}$";

        protected MritMapper(OscarContext oscarContext, VwOnMusicFelixWorks felixWork)
        {
            OscarCOntext = oscarContext;
            FelixWork = felixWork;
            MritProductionModel = new ProductionModel();
        }

        protected abstract Task Process();

        public async Task<ProductionModel> MapFrom()
        {
            var titles = FelixWork.Titles != null ? JsonConvert.DeserializeObject<IEnumerable<TitleModel>>(FelixWork.Titles)
                : new List<TitleModel>()
                {
                    new() {Title = "No title", LanguageCode = "ENG"}
                };

            foreach (var titleModel in titles)
            {
                titleModel.Title = titleModel.Title.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title);
            }

            MritProductionModel.Id = FelixWork.WorksId;
            MritProductionModel.EnglishTitle = titles!.First().Title;
            MritProductionModel.CompactRef = FelixWork.CompactRef;
            MritProductionModel.Episodes = new List<EpisodeModel>();
            MritProductionModel.People = new List<PersonModel>();
            MritProductionModel.Companies = FelixWork.GetCompaniesFrom();
            MritProductionModel.Duration = FelixWork.Duration;
            MritProductionModel.Genres = FelixWork.GetGenresFrom();
            MritProductionModel.Countries = FelixWork.GetCountriesFrom();
            MritProductionModel.Languages = FelixWork.GetLanguagesFrom(OscarCOntext.Languages);
            MritProductionModel.Names = titles;
            MritProductionModel.ISAN = FelixWork.Isan;
            MritProductionModel.Date = FelixWork.ProductionYear == null || !Regex.IsMatch(FelixWork.ProductionYear.ToString(), DatePattern)
                ? null : new DateTime(FelixWork.ProductionYear.Value,1,1);

            await Process();

            return MritProductionModel;
        }
    }
}
