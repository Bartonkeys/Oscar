using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;
using Oscar.MRIT.Core.Constants;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.Mrit.Features.MRITIntegration.Common
{
    public static class FelixWorksQueryExtensions
    {
        public static IEnumerable<LanguageModel> GetLanguagesFrom(this VwOnMusicFelixWorks felixWork, DbSet<Language> languages) =>
            languages.Where(l => l.Works.Any(w => w.Id == felixWork.WorksId)).Select(w => new LanguageModel { ISO639_2 = w.Name.ToLower() }).ToList();

        public static IEnumerable<CountryModel> GetCountriesFrom(this VwOnMusicFelixWorks felixWork) =>
            new List<CountryModel> { new CountryModel { CountryCode = felixWork.Nationality } };

        public static IEnumerable<GenreModel> GetGenresFrom(this VwOnMusicFelixWorks felixWork)
        {
            var genres = new List<GenreModel>();

            if (felixWork.Genre == FelixConstants.Genre.Fiction)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Fiction });

            if (felixWork.Genre == FelixConstants.Genre.Animation)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Animation });

            if (felixWork.Genre == FelixConstants.Genre.Unknown)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Unknown });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Factual });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Reality)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Reality });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Infomercial)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Informational });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Sport)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Sport });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Music)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Music });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Comedy)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.Comedy });

            if (felixWork.Genre == FelixConstants.Genre.NonFiction && felixWork.GenreSubType == FelixConstants.GenreSubType.Children)
                genres.Add(new GenreModel { MainName = MritConstants.Genre.FactualChildren });

            return genres;
        }

        public static IEnumerable<CompanyModel> GetCompaniesFrom(this VwOnMusicFelixWorks felixWork)
        {
            var companies = string.IsNullOrEmpty(felixWork.ProductionCompanies)
                ? new List<string>()
                : felixWork.ProductionCompanies.Split("|").ToList();

            return companies.Select(company => new CompanyModel { MainName = company.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title) }).ToList();
        }

        public static IEnumerable<PersonModel> GetPeopleFrom(this VwOnMusicFelixWorks felixWork)
        {
            var actors = string.IsNullOrEmpty(felixWork.Actors) ? new List<string>() : felixWork.Actors.Split('|').ToList();
            var directors = string.IsNullOrEmpty(felixWork.Directors) ? new List<string>() : felixWork.Directors.Split('|').ToList();
            var producers = string.IsNullOrEmpty(felixWork.Producers) ? new List<string>() : felixWork.Producers.Split('|').ToList();
            var people = actors.Select(actor => new PersonModel { MainName = actor.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title), Type = "Actor" }).ToList();
            people.AddRange(directors.Select(director => new PersonModel { MainName = director.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title), Type = "Director" }));
            people.AddRange(producers.Select(producer => new PersonModel { MainName = producer.Humanize(LetterCasing.LowerCase).Humanize(LetterCasing.Title), Type = "Producer" }));

            return people;
        }
    }
}
