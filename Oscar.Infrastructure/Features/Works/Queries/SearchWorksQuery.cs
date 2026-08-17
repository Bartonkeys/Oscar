using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Works.Builders;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class SearchWorksQuery : IRequest<Result<IEntityTable<WorksDto>>>
    {
        public int Start { get; set; } = 0;
        public int Take { get; set; } = 20;
        public string SortColumn { get; set; } = "Id";
        public SearchDirection SortDirection { get; set; } = SearchDirection.Ascending;
        public List<Discriminator> Discriminators { get; set; } = new List<Discriminator>();
        public string? Title { get; set; }
        public string? ActorFirstName { get; set; }
        public string? ActorLastName { get; set; }
        public string? ProducerFirstName { get; set; }
        public string? ProducerLastName { get; set; }
        public string? DirectorFirstName { get; set; }
        public string? DirectorLastName { get; set; }
        public string? ScreenWriterFirstName { get; set; }
        public string? ScreenWriterLastName { get; set; }
        public int? ProductionYear { get; set; }
        public int? ClientID { get; set; }
        public int? CatalogueId { get; set; }
        public WorksStatus StatusDiscriminator { get; set; }
        public int? CountryID { get; set; }
        public int? RightsCountryID { get; set; }
        public SearchType SearchType { get; set; } = SearchType.Contains;
        public int? FirstBroadcastYear { get; set; }
        public int? WorksTypeId { get; set; }
        public bool? HasNoRights { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public string? SearchStringAgicoaRef { get; set; }
        public string? SearchStringCompactRef { get; set; }
        public string? SearchStringAS400 { get; set; }
        public int? ProductionCompanyID { get; set; }
        public bool IncludeAlternateTitles { get; set; } = true;

        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(Title)) ||
                       (!string.IsNullOrWhiteSpace(ActorFirstName)) ||
                       (!string.IsNullOrWhiteSpace(ActorLastName)) ||
                       (!string.IsNullOrWhiteSpace(ProducerFirstName)) ||
                       (!string.IsNullOrWhiteSpace(ProducerLastName)) ||
                       (!string.IsNullOrWhiteSpace(DirectorFirstName)) ||
                       (!string.IsNullOrWhiteSpace(DirectorLastName)) ||
                       (!string.IsNullOrWhiteSpace(ScreenWriterFirstName)) ||
                       (!string.IsNullOrWhiteSpace(ScreenWriterLastName)) ||
                       (!string.IsNullOrWhiteSpace(SearchStringAS400)) ||
                       (!string.IsNullOrWhiteSpace(SearchStringAgicoaRef)) ||
                       (!string.IsNullOrWhiteSpace(SearchStringCompactRef)) ||
                       (ProductionCompanyID.HasValue || ProductionYear.HasValue ||
                       ClientID.HasValue || CatalogueId.HasValue || CountryID.HasValue ||
                       RightsCountryID.HasValue || FirstBroadcastYear.HasValue ||
                       WorksTypeId.HasValue || HasNoRights.HasValue);
            }
        }
    }

    public class SearchWorksQueryHandler : AbstractBaseHandler<SearchWorksQuery, IEntityTable<WorksDto>>
    {
        private readonly IEnumerable<IPredicateBuilder> _predicateBuilders =
            new List<IPredicateBuilder>
            {
                new ActorPredicateBuilder(),
                new DirectorPredicateBuilder(),
                new ProducerPredicateBuilder(),
                new ScreenWriterPredicateBuilder(),
                new ProductionYearPredicateBuilder(),
                new BroadcastYearPredicateBuilder(),
                new RightsCountryPredicateBuilder(),
                new WorksTypePredicateBuilder(),
                new HasNoRightsPredicateBuilder(),
                new DateCreatedFromPredicateBuilder(),
                new DateCreatedToPredicateBuilder(),
                new AgicoaRefPredicateBuilder(),
                new CompactRefPredicateBuilder(),
                new AS400PredicateBuilder(),
                new ProductionCompanyPredicateBuilder()
            };

        public SearchWorksQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<SearchWorksQuery> validator, ILogger<SearchWorksQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<WorksDto>>> HandleRequest(SearchWorksQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)WorksFeatureEvent.Get, "GET");

            if (!request.CountryID.HasValue) { request.CountryID = 0; }

            if (request.Take <= 0) { request.Take = Constants.Default.MaxRecordsFetchCount; }

            IQueryable<Core.Entities.Works> works = OscarContext.Works.AsExpandable().AsNoTracking()
                .Include(i => i.WorksType)
                .Include(i => i.WorksSubType)
                .Include(t => t.Titles)
                .Include(i => i.Genre)
                .Include(c => c.Clients)
                .Include(c => c.Catalogues)
                .Include(c => c.Directors)
                .Include(c => c.Actors)
                .Include(c => c.Producers)
                .Include(c => c.ScreenWriters)
                .Include(c => c.Companies)
                .Where(BuildPredicate(request));

            List<WorksDto>? worksdto = null;
            int totalRecords = 0;

            if (!string.IsNullOrEmpty(request.Title))
            {
                worksdto = await WorksSearchByTitle(works, request);
            }
            else
            {
                worksdto = works.ToList().Select(c => Mapper.Map<WorksDto>(c)).ToList();
            }

            totalRecords = worksdto.Count();
            worksdto = request.SortDirection == SearchDirection.Ascending ? worksdto.OrderByDynamic(c => $"c.{request.SortColumn}").ToList() : worksdto.OrderByDescendingDynamic(c => $"c.{request.SortColumn}").ToList();

            Logger.LogInformation((int)WorksFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<WorksDto>.Create(worksdto).WithTotal(totalRecords));
        }

        private async Task<List<WorksDto>> WorksSearchByTitle(IQueryable<Core.Entities.Works> works, SearchWorksQuery request)
        {
            if (string.IsNullOrEmpty(request.Title))
                return new List<WorksDto>();

            var worksTitleResults = await GetWorksTitleByTitleAsync(request.Title, request.SearchType.ToString());

            if (worksTitleResults == null || !worksTitleResults.Any())
                return new List<WorksDto>();

            var worksIds = worksTitleResults.Select(wt => wt.WorksId).ToHashSet();

            //Query database
            var worksQuery = await works
                .Where(w => worksIds.Contains(w.Id))
                .ToListAsync();

            //Note: Mapping to WorksDto defaults DisplayTitleType to Main
            var worksDtos = worksQuery.Select(w => Mapper.Map<WorksDto>(w)).ToList();

            var alternateTitles = worksTitleResults
                .Where(y => y.TitleType == (int)TitleType.MainAlternative || y.TitleType == (int)TitleType.EpisodeAlternative)
                .ToList();

            //Duplicate works dto for each matching title found in alternate titles from worksTitleResults list as the user wants to see duplicate works
            //in search resultset against all the alternate titles associated with respective work
            foreach (var title in alternateTitles)
            {
                var originalDto = worksDtos.FirstOrDefault(x => x.Id == title.WorksId);
                if (originalDto != null)
                {
                    var alternateDto = CloneHelper.Clone(originalDto);
                    alternateDto.DisplayTitle = title.Title;
                    alternateDto.DisplayTitleType = (TitleType)title.TitleType;
                    worksDtos.Add(alternateDto);
                }
            }

            var mainTitles = worksTitleResults
                .Where(y => y.TitleType == (int)TitleType.Main || y.TitleType == (int)TitleType.Episode)
                .ToDictionary(y => y.WorksId);

            //Remove main titles from worksDtos list if no matching main titles are found in worksTitleResults list
            //Replace DisplayTitle with the matching main titles from worksTitleResults list
            worksDtos = worksDtos.Where(dto =>
            {
                if (dto.DisplayTitleType == TitleType.Main || dto.DisplayTitleType == TitleType.Episode)
                {
                    if (mainTitles.TryGetValue(dto.Id, out var mainTitle))
                    {
                        dto.DisplayTitle = mainTitle.Title;
                        dto.DisplayTitleType = (TitleType)mainTitle.TitleType;
                        return true;
                    }
                    return false;
                }
                return true;
            }).ToList();

            return worksDtos;
        }

        private Expression<Func<WorksTitle, bool>> BuildTitlePredicate(SearchWorksQuery request)
        {
            var predicate = PredicateBuilder.New<WorksTitle>(true);
            if (!string.IsNullOrEmpty(request.Title))
            {
                switch (request.SearchType)
                {
                    case SearchType.FreeText:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.FreeText(t.Title, $"{request.Title}"));
                        break;
                    case SearchType.Contains:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.Contains(t.Title, $"{request.Title}"));
                        break;
                    case SearchType.ContainsExpression:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && (EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearch(request.Title)) ||
                                                        EF.Functions.Contains(t.Title, QueryHelpers.BuildContainsFullTextSearchPrefix(request.Title))));
                        break;
                    case SearchType.StartsWith:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.Like(t.Title, $"{request.Title}%"));
                        break;
                    case SearchType.Like:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && EF.Functions.Like(t.Title, $"%{request.Title}%"));
                        break;
                    case SearchType.Equals:
                        predicate = predicate.And(t => (request.IncludeAlternateTitles ? 1 == 1 : (t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)) && t.Title.Equals(request.Title));
                        break;
                }
            }
            return predicate;
        }

        public Expression<Func<Core.Entities.Works, bool>> BuildPredicate(SearchWorksQuery request)
        {
            var predicate = PredicateBuilder.New<Core.Entities.Works>(true);

            if (request.ClientID != null && request.ClientID > 0)
                predicate = predicate.And(c => c.Clients.Any(c => c.Id == request.ClientID));

            if (request.CatalogueId != null && request.CatalogueId > 0)
                predicate = predicate.And(c => c.Catalogues.Any(c => c.Id == request.CatalogueId));

            foreach (var predicateBuilder in _predicateBuilders)
                predicate = predicateBuilder.Build(request, predicate);

            predicate = BuildDiscriminatorPredicates(request, predicate);

            if (request.StatusDiscriminator != WorksStatus.Any)
                predicate = predicate.And(c => c.WorksStatus == request.StatusDiscriminator);

            if (request.StatusDiscriminator != WorksStatus.Uncontrolled)
                predicate = predicate.And(c => c.WorksStatus != WorksStatus.Uncontrolled);

            return predicate;
        }

        private static ExpressionStarter<Core.Entities.Works> BuildDiscriminatorPredicates(SearchWorksQuery request, ExpressionStarter<Core.Entities.Works> predicate)
        {
            if (request.Discriminators.Contains(Discriminator.All))
                return predicate;

            bool isFirstDiscriminatorItem = true;
            var discriminatorPredicate = PredicateBuilder.New<Core.Entities.Works>(true);

            foreach (var discriminatorItem in request.Discriminators)
            {
                if (isFirstDiscriminatorItem)
                {
                    switch (discriminatorItem)
                    {
                        case Discriminator.Series:
                            discriminatorPredicate = discriminatorPredicate.And(c => c.Discriminator == Discriminator.Series.ToString());
                            break;
                        case Discriminator.Season:
                            discriminatorPredicate = discriminatorPredicate.And(c => c.Discriminator == Discriminator.Season.ToString());
                            break;
                        case Discriminator.Episode:
                            discriminatorPredicate = discriminatorPredicate.And(c => c.Discriminator == Discriminator.Episode.ToString());
                            break;
                        case Discriminator.StandAlone:
                            discriminatorPredicate = discriminatorPredicate.And(c => c.Discriminator == Discriminator.StandAlone.ToString());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (discriminatorItem)
                    {
                        case Discriminator.Series:
                            discriminatorPredicate = discriminatorPredicate.Or(c => c.Discriminator == Discriminator.Series.ToString());
                            break;
                        case Discriminator.Season:
                            discriminatorPredicate = discriminatorPredicate.Or(c => c.Discriminator == Discriminator.Season.ToString());
                            break;
                        case Discriminator.Episode:
                            discriminatorPredicate = discriminatorPredicate.Or(c => c.Discriminator == Discriminator.Episode.ToString());
                            break;
                        case Discriminator.StandAlone:
                            discriminatorPredicate = discriminatorPredicate.Or(c => c.Discriminator == Discriminator.StandAlone.ToString());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                isFirstDiscriminatorItem = false;
            }

            predicate = predicate.And(discriminatorPredicate);

            return predicate;
        }

        public async Task<List<WorksTitleResult>> GetWorksTitleByTitleAsync(string title, string searchType)
        {
            return await OscarContext.WorksTitleResults
                .FromSqlInterpolated($"[dbo].[sp_GetWorksTitleByTitle] {title}, {searchType}")
                .ToListAsync();

        }
    }
}
