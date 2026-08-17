using FuzzySharp;

namespace Oscar.Infrastructure.Features.Matching.Services
{
    internal static class MatchHelper
    {
        internal static bool DirectorMismatch(Oscar.Core.Entities.Works works, string? directorOne, string? directorTwo)
        {
            if(directorOne == null && directorTwo == null) return false;

            var directorOneFound = directorOne != null && works.Directors.Any(d => Fuzz.TokenSetRatio($"{d.FirstName} {d.LastName}".ToLower(), directorOne.ToLower()) > 90);
            var directorTwoFound = directorTwo != null && works.Directors.Any(d => Fuzz.TokenSetRatio($"{d.FirstName} {d.LastName}".ToLower(), directorTwo.ToLower()) > 90);
            
            return (directorOneFound == false && directorTwoFound == false) ;
        }

        internal static bool RightsTypeMismatch(Oscar.Core.Entities.Works works, int? rightsTypeId, int? clientId)
        {
            return rightsTypeId != null 
                   && (works?.Rights == null 
                       || works.Rights.Any(r => r.Type.Id == rightsTypeId) == false);
        }

        internal static bool RightsYearsMismatch(Oscar.Core.Entities.Works works, int? rightsFromYear, int? rightsToYear, int? clientId)
        {
            return rightsFromYear != null && rightsToYear != null 
                                          && (works?.Rights == null || works.Rights.Any(r => r.StartOfRight.Year == rightsFromYear && r.EndOfRight.Year == rightsToYear) == false);
        }

        internal static bool ProductionYearMismatch(Oscar.Core.Entities.Works works, int? productionYear)
        {
            return productionYear != null && Math.Abs(works.ProductionYear.GetValueOrDefault() - productionYear.Value) > 1;
        }

        internal static bool TerritoryMismatch(Oscar.Core.Entities.Works works, int? territoryId, int? clientId)
        {
            var isNotInCountries = territoryId != null
                                && works?.Rights?.Any(r => r.Countries != null && r.Countries.Any(t => t.Id == territoryId)) == false;

            return isNotInCountries;
        }

        internal static bool DurationMismatch(Core.Entities.Works works, string? duration)
        {
            if (works.DurationMinutes == null || !int.TryParse(duration, out int durationMinutes)) return false;

            return Math.Abs(durationMinutes - works.DurationMinutes.GetValueOrDefault()) > 10;
        }

        internal static bool ProductionCountryMismatch(Core.Entities.Works works, string[]? productionCountry)
        {
            if(productionCountry == null || productionCountry.Count() == 0) return false;
            if (works.Countries == null) return true;

            foreach (var country in productionCountry)
            {
                if(works.Countries.Any(c => c.Name == country))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string? IgnoreCharactersFollowing(string? title, string removeFrom)
        {
            if (title == null) return null;

            var position = title.IndexOf(removeFrom);
            if(position > 0)
            {
                title = title.Substring(0, position);
            }
            return title;
        }
    }
}
