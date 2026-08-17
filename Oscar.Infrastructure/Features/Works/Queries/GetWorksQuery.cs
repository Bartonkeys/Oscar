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
using Microsoft.EntityFrameworkCore;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetWorksQuery: BaseTableQuery, IRequest<Result<IEntityTable<WorksDto>>>
    {
        public GetWorksQuery()
        {
            BaseEntityName = "Works";
            SortDirection = "ascending";
        }
    }

    public class WorksHandler : AbstractBaseHandler<GetWorksQuery, IEntityTable<WorksDto>>
    {
        readonly IDynamicExpressionBuilderService _dynamicExpressionBuilderService;

        public WorksHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksQuery> validator, ILogger<GetWorksQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService) : base(oscarContext, mapper, validator, logger)
        {
            _dynamicExpressionBuilderService = dynamicExpressionBuilderService;
        }

        protected override async Task<Result<IEntityTable<WorksDto>>> HandleRequest(GetWorksQuery request, CancellationToken cancellationToken)
        {
            var works = GetWorks(request);
            var total = await works.CountAsync(cancellationToken);
            var pagedWorks = works
                .AsNoTracking()
                .Include(w => w.Titles)
                .AsSplitQuery()
                .Skip(request.Start)
                .Take(request.Take);

            Logger.LogInformation((int)WorksFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<WorksDto>.Create(pagedWorks.Select(c => Mapper.Map<WorksDto>(c))).WithTotal(total));
        }

        private IOrderedQueryable<Oscar.Core.Entities.Works> GetWorks(GetWorksQuery request) =>
            request.SortDirection.Equals("descending", StringComparison.OrdinalIgnoreCase)
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Oscar.Core.Entities.Works> BuildQuery(GetWorksQuery request) =>
            OscarContext.Works
                .WhereDynamic(BuildPredicate(request));

        private Expression<Func<Oscar.Core.Entities.Works, string>> BuildOrderByExpression(GetWorksQuery request) =>
            c => $"c.{_dynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.WorksKnownColumns)}";
        
        private Expression<Func<Oscar.Core.Entities.Works, string>> BuildPredicate(GetWorksQuery request) =>
            c => _dynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.WorksKnownColumns);

    }
}
