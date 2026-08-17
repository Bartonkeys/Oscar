using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using System.Linq.Expressions;
using Oscar.Infrastructure.Features.Common.Contracts;


namespace Oscar.Infrastructure.Features.Report.Queries
{
    public class GetReportsQuery: BaseTableQuery, IRequest<Result<IEntityTable<ReportDto>>>
    {
        public GetReportsQuery()
        {
            BaseEntityName = "Report";

        }

    }

    public class ReportsHandler : AbstractBaseHandler<GetReportsQuery, IEntityTable<ReportDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        public ReportsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetReportsQuery> validator, ILogger<GetReportsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
        }

        protected override async Task<Result<IEntityTable<ReportDto>>> HandleRequest(GetReportsQuery request, CancellationToken cancellationToken)
        {
            var reports = GetReports(request).ToList();
            var total = reports.Count();
            var pagedReports = reports
                .Skip(request.Start)
                .Take(request.Take);


            Logger.LogInformation((int)ReportFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(EntityTable<ReportDto>.Create(pagedReports.Select(c => Mapper.Map<ReportDto>(c))).WithTotal(total));
        }

        private IOrderedQueryable<Core.Entities.Report> GetReports(GetReportsQuery request)=>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Core.Entities.Report> BuildQuery(GetReportsQuery request) =>
            OscarContext.Reports
                .WhereDynamic(BuildPredicate(request))
                .Include(x => x.ReportFields);
        
        private Expression<Func<Core.Entities.Report, string>> BuildOrderByExpression(GetReportsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.ReportKnownColumns)}";

        private Expression<Func<Core.Entities.Report, string>> BuildPredicate(GetReportsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.ReportKnownColumns);

    }
}
