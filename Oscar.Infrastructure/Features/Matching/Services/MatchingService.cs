using System.Collections.Concurrent;
using System.Text;
using AutoMapper;
using BartonKeys.Functional;
using FuzzySharp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Matching.Contracts;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.Features.Matching.Services
{
    public class MatchingService : IMatchingService
    {
        private ConcurrentBag<WorksTitles>? _worksList;
        private ConcurrentBag<Oscar.Core.Entities.Client> _clientList;
        private IMapper _mapper;

        private MatchRules _rules;
        private OscarContext _oscarContext;
        private int? _clientId;
        private int? _territoryId;
        private int? _rightsTypeId;
        private int? _rightsFromYear;
        private int? _rightsToYear;
        private string _ignoreCharactersFollowing;
        private readonly ILogger<IMatchingService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;

        public MatchingService(OscarContext oscarContext, IMapper mapper,
            ILogger<IMatchingService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IMediator mediator)
        {
            _rules = MatchRules.None;
            _oscarContext = oscarContext;
            _mapper = mapper;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _mediator = mediator;
        }

        public void LoadRules(
            MatchRules rules,
            int? clientId,
            int? territoryId,
            int? rightsTypeId,
            int? rightsFromYear,
            int? rightsToYear,
            string ignoreCharactersFollowing)
        {
            _rules = rules;
            _clientId = clientId;
            _territoryId = territoryId;
            _rightsTypeId = rightsTypeId;
            _rightsFromYear = rightsFromYear;
            _rightsToYear = rightsToYear;
            _ignoreCharactersFollowing = ignoreCharactersFollowing;

            if (_clientId == null)
            {
                _clientList = new ConcurrentBag<Client>(_oscarContext.Clients.ToList());
            }

            //_worksList = new ConcurrentBag<WorksTitles>(_oscarContext.Works.AsNoTracking().Where(w =>
            //    w is Oscar.Core.Entities.StandAlone || 
            //        (_rules.HasFlag(MatchRules.SeriesTitle) && w is Oscar.Core.Entities.Series) ||
            //        (_rules.HasFlag(MatchRules.EpisodeTitle) && w is Oscar.Core.Entities.Episode))
            //    .Select(w => new WorksTitles() { Id = w.Id, Titles = w.Titles == null ?  new List<string>() : w.Titles.Select(t => t.Title == null ? "" : t.Title.ToLower()).ToList() })
            //    .ToList());

        }

        public async Task<Result<MatchTemplateResultsDto>> Match(MatchTemplateDto matchDto)
        {
            _logger.LogInformation($"MATCH SERVICE Begin match progress on {matchDto.Title1} on {System.Environment.CurrentManagedThreadId}");

            //if (_worksList == null)
            //{
            //    throw new MatchingServiceRulesNotSetException("Attempted to call Match without first calling LoadRules");
            //}

            if (_clientId == null && matchDto.ClientReference != null && int.TryParse(matchDto.ClientReference, out var clientReference))
            {
                var client = _clientList.FirstOrDefault(c => c.ClientReference == clientReference);
                if (client != null)
                {
                    _clientId = client.Id;
                }
            }

            if (_rules.HasFlag(MatchRules.IgnoreCharactersFollowing) && _ignoreCharactersFollowing != null)
            {
                matchDto.Title1 = MatchHelper.IgnoreCharactersFollowing(matchDto.Title1, _ignoreCharactersFollowing);
                matchDto.Title2 = MatchHelper.IgnoreCharactersFollowing(matchDto.Title2, _ignoreCharactersFollowing);
                matchDto.Title3 = MatchHelper.IgnoreCharactersFollowing(matchDto.Title3, _ignoreCharactersFollowing);
            }

            var matchTemplateResultsDto = _mapper.Map<MatchTemplateResultsDto>(matchDto);
            matchTemplateResultsDto.ProductionCountry = String.Join(";", matchDto.ProductionCountry);

            //var searchResultsInMemory = _worksList.Where(w =>
            //    (w.Titles != null && matchDto.Title1 != null && w.Titles.Any(t => Fuzz.TokenSetRatio(t, matchDto.Title1.ToLower()) == 100)) ||
            //    (_rules.HasFlag(MatchRules.TitleCheckLevel2) && w.Titles != null && matchDto.Title2 != null && w.Titles.Any(t => Fuzz.TokenSetRatio(t, matchDto.Title2.ToLower()) == 100)) ||
            //    (_rules.HasFlag(MatchRules.TitleCheckLevel3) && w.Titles != null && matchDto.Title3 != null && w.Titles.Any(t => Fuzz.TokenSetRatio(t, matchDto.Title3.ToLower()) == 100)))
            //    .ToList();

            var searchResults = await _mediator.Send(new SearchWorksQuery
            {
                Title = BuildTitleContainsString(matchDto),
                Discriminators = new List<Discriminator> { GetDiscriminator() },
                StatusDiscriminator = WorksStatus.Any,
                SortColumn = "Id",
                SearchType = SearchType.Contains,
                ProductionYear = _rules.HasFlag(MatchRules.ProductionYear) && !string.IsNullOrEmpty(matchDto.ProductionYear) ? int.Parse(matchDto.ProductionYear) : null,
                FirstBroadcastYear = _rules.HasFlag(MatchRules.FirstBroadcastYear) && !string.IsNullOrEmpty(matchDto.BroadcastDate) ? int.Parse(matchDto.BroadcastDate.Substring(0, 4)) : null,
                RightsCountryID = _rules.HasFlag(MatchRules.RightsCountry) ? _territoryId : null,
            }).ConfigureAwait(false);

            var worksListCount = searchResults.Value.TotalRecords;
            switch (worksListCount)
            {
                case > 1:
                    matchTemplateResultsDto.MatchingIssue = "Multiple titles found";
                    break;
                case 0:
                    matchTemplateResultsDto.MatchingIssue = "No match found";
                    break;
                default:
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var scopedServices = scope.ServiceProvider;
                        var oscarContextThreadSafe = scopedServices.GetRequiredService<OscarContext>();

                        var worksId = searchResults.Value.Records.FirstOrDefault()?.Id;
                        var works = await oscarContextThreadSafe.Works
                            .Include(w => w.Titles)
                            .Include(w => w.Directors)
                            .Include(w => w.Clients)!.ThenInclude(r => r.Contacts)
                            .Include(e => e.Rights).ThenInclude(t => t.Type)
                            .Include(r => r.Rights).ThenInclude(c => c.Countries)
                            .AsNoTracking()
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(w => w.Id == worksId);

                        if (works != null)
                        {
                            var client = works.Clients.FirstOrDefault();
                            matchTemplateResultsDto.OscarClient = client?.ClientName;
                            matchTemplateResultsDto.ClientEndDate = client?.Contract?.EndDate?.ToString("dd/MM/yyyy");
                            matchTemplateResultsDto.OscarDirector = string.Join(";", works.Directors.Select(d => d.FirstName + " " + d.LastName));
                            matchTemplateResultsDto.OscarProductionYear = works.ProductionYear.ToString();
                            matchTemplateResultsDto.MatchingIssue = GetMatchingIssues(works, matchDto);
                        }

                        break;
                    }
            }

            _logger.LogInformation($"MATCH SERVICE Finish match progress on {matchDto.Title1} on {System.Environment.CurrentManagedThreadId}");
            return Result.Ok(matchTemplateResultsDto);
        }

        private string? BuildTitleContainsString(MatchTemplateDto matchDto)
        {

            var result = new StringBuilder($"\"{matchDto.Title1.CleanseOf('"')}\"");

            if (_rules.HasFlag(MatchRules.TitleCheckLevel2) && !string.IsNullOrEmpty(matchDto.Title2))
                result.Append($" OR \"{matchDto.Title2.CleanseOf('"')}\"");

            if (_rules.HasFlag(MatchRules.TitleCheckLevel3) && !string.IsNullOrEmpty(matchDto.Title3))
                result.Append($" OR \"{matchDto.Title3.CleanseOf('"')}\"");

            return result.ToString();
        }

        private Discriminator GetDiscriminator()
        {
            if (!_rules.HasFlag(MatchRules.SeriesTitle) && !_rules.HasFlag(MatchRules.EpisodeTitle))
                return Discriminator.StandAlone;

            if (_rules.HasFlag(MatchRules.SeriesTitle) && !_rules.HasFlag(MatchRules.EpisodeTitle))
                return Discriminator.Series;

            if (!_rules.HasFlag(MatchRules.SeriesTitle) && _rules.HasFlag(MatchRules.EpisodeTitle))
                return Discriminator.Episode;

            return Discriminator.All;
        }

        private string GetMatchingIssues(Oscar.Core.Entities.Works works, MatchTemplateDto matchDto)
        {
            var matchingIssues = new List<string>();

            //if (_rules.HasFlag(MatchRules.Territory) && MatchHelper.TerritoryMismatch(works, _territoryId, _clientId))
            //{
            //    matchingIssues.Add(Mismatch.TerritoryRightsMismatch);
            //}

            //if (_rules.HasFlag(MatchRules.ProductionYear) && MatchHelper.ProductionYearMismatch(works, _productionYear))
            //{
            //    matchingIssues.Add(Mismatch.ProductionYearMismatch);
            //}

            if (_rules.HasFlag(MatchRules.RightsYears) && MatchHelper.RightsYearsMismatch(works, _rightsFromYear, _rightsToYear, _clientId))
            {
                matchingIssues.Add(Mismatch.RightsFromAndToYearMismatch);
            }

            if (_rules.HasFlag(MatchRules.RightsType) && MatchHelper.RightsTypeMismatch(works, _rightsTypeId, _clientId))
            {
                matchingIssues.Add(Mismatch.RightsTypeMismatch);
            }

            if (_rules.HasFlag(MatchRules.Director) && MatchHelper.DirectorMismatch(works, matchDto.Director1, matchDto.Director2))
            {
                matchingIssues.Add(Mismatch.DirectorMismatch);
            }

            if (_rules.HasFlag(MatchRules.Duration) && MatchHelper.DurationMismatch(works, matchDto.Duration))
            {
                matchingIssues.Add(Mismatch.DurationMismatch);
            }

            if (_rules.HasFlag(MatchRules.ProductionCountry) && MatchHelper.ProductionCountryMismatch(works, matchDto.ProductionCountry))
            {
                matchingIssues.Add(Mismatch.ProductionCountryMismatch);
            }

            return string.Join("; ", matchingIssues);
        }

    }


    public class MatchingServiceRulesNotSetException : Exception
    {
        public MatchingServiceRulesNotSetException() : base()
        {
        }

        public MatchingServiceRulesNotSetException(string message)
            : base(message)
        {
        }

        public MatchingServiceRulesNotSetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class Mismatch
    {
        public static string TerritoryRightsMismatch = "Territory rights mismatch";
        public static string ProductionYearMismatch = "Production year mismatch";
        public static string RightsFromAndToYearMismatch = "Rights from and to year mismatch";
        public static string RightsTypeMismatch = "Rights type mismatch";
        public static string DirectorMismatch = "Director mismatch";
        public static string DurationMismatch = "Duration mismatch";
        public static string ProductionCountryMismatch = "Production country mismatch";
    }

    internal class WorksTitles
    {
        public int Id { get; set; }
        public List<string> Titles { get; set; }
    }
}
