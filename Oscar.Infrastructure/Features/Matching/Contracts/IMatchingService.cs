using BartonKeys.Functional;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;

namespace Oscar.Infrastructure.Features.Matching.Contracts;

public interface IMatchingService
{
    void LoadRules(
            MatchRules rules,
            int? clientId,
            int? territoryId,
            int? rightsTypeId,
            int? rightsFromYear,
            int? rightsToYear,
            string ignoreCharactersFollowing);

    Task<Result<MatchTemplateResultsDto>> Match(MatchTemplateDto matchDto);
}