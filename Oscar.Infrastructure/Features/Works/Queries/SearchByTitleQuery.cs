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
using Oscar.Infrastructure.Features.Series.Queries;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class SearchByTitleQuery : IRequest<Result<IEntityTable<WorksDto>>>
    {
        public int Start { get; set; } = 0;
        public int Take { get; set; } = 20;
        public string? Title { get; set; }
        public Discriminator Discriminator { get; set; }
        public SearchType SearchType { get; set; } = SearchType.Like;
        public WorksStatus WorksStatus { get; set; } = WorksStatus.Any;
        public SearchDirection SearchDirection { get; set; } = SearchDirection.Ascending;
    }

    public class SearchByTitleQueryHandler : AbstractBaseHandler<SearchByTitleQuery, IEntityTable<WorksDto>>
    {
        public SearchByTitleQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<SearchByTitleQuery> validator, ILogger<SearchByTitleQuery> logger)
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<WorksDto>>> HandleRequest(SearchByTitleQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Title))
                return Result.Ok(EntityTable<WorksDto>.Create(new List<WorksDto>()).WithTotal(0));

            var titles = OscarContext
                .WorksTitles
                .AsExpandable()
                .AsNoTracking()
                .Include(t => t.Works).ThenInclude(c => c.Catalogues)
                .Include(t => t.Works).ThenInclude(c => c.Clients)
                .Where(BuildTitlePredicate(request));

            IOrderedQueryable<WorksTitle> orderedTitles = null;
            switch (request.SearchDirection)
            {
                case SearchDirection.Ascending:
                    orderedTitles = titles.OrderBy(t => t.Title);
                    break;
                case SearchDirection.Descending:
                    orderedTitles = titles.OrderByDescending(t => t.Title);
                    break;
            }

            var total = orderedTitles.Select(p => p.Id).Count();
            var pagedWorks = orderedTitles.Skip(request.Start).Take(request.Take).Select(t => Mapper.Map<WorksDto>(t));

            Logger.LogInformation((int)WorksFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<WorksDto>.Create(pagedWorks).WithTotal(total));
        }

        public Expression<Func<WorksHeader, bool>> BuildPredicate(SearchByTitleQuery request)
        {
            var predicate = PredicateBuilder.New<WorksHeader>(true);
            predicate = request.SearchType.BuildSearchTypePredicate(predicate, request.Title);

            return predicate;
        }

        public Expression<Func<WorksTitle, bool>> BuildTitlePredicate(SearchByTitleQuery request)
        {
            var predicate = PredicateBuilder.New<WorksTitle>(true);
            predicate = request.SearchType.BuildSearchTypePredicate(predicate, request.Title);

            switch (request.Discriminator)
            {
                case Discriminator.All:
                    break;
                case Discriminator.Series:
                    predicate = predicate.And(c => c.Works.Discriminator == Discriminator.Series.ToString());
                    break;
                case Discriminator.Season:
                    predicate = predicate.And(c => c.Works.Discriminator == Discriminator.Season.ToString());
                    break;
                case Discriminator.Episode:
                    predicate = predicate.And(c => c.Works.Discriminator == Discriminator.Episode.ToString());
                    break;
                case Discriminator.StandAlone:
                    predicate = predicate.And(c => c.Works.Discriminator == Discriminator.StandAlone.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (request.WorksStatus != WorksStatus.Any)
                predicate = predicate.And(c => c.Works.WorksStatus == request.WorksStatus);

            if (request.WorksStatus != WorksStatus.Uncontrolled)
                predicate = predicate.And(c => c.Works.WorksStatus != WorksStatus.Uncontrolled);

            return predicate;
        }

    }
}
