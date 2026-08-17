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
using Oscar.Infrastructure.Features.Common.Contracts;
using Microsoft.Extensions.Configuration;


namespace Oscar.Infrastructure.Features.Screenrights.Queries
{
    public class GetScreenrightsRequestsQuery : BaseTableQuery, IRequest<Result<IEntityTable<ScreenrightsRequestDto>>>
    {
        public GetScreenrightsRequestsQuery()
        {
            BaseEntityName = "ScreenrightsRequest";
        }
    }

    public class ScreenrightsRequestsQueryHandler : AbstractBaseHandler<GetScreenrightsRequestsQuery, IEntityTable<ScreenrightsRequestDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        private readonly IConfiguration config;


        public ScreenrightsRequestsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetScreenrightsRequestsQuery> validator, ILogger<GetScreenrightsRequestsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
            config = configuration;

        }

        protected override async Task<Result<IEntityTable<ScreenrightsRequestDto>>> HandleRequest(GetScreenrightsRequestsQuery request, CancellationToken cancellationToken)
        {
            var screenrightsRequests = GetScreenrightsRequests(request);
            var total = screenrightsRequests.Count();
            var pagedScreenrightsRequests = screenrightsRequests
                .Skip(request.Start)
                .Take(request.Take);


            var screenrightsRequestDtos = pagedScreenrightsRequests.Select(c => Mapper.Map<ScreenrightsRequestDto>(c)).ToList();
            foreach (var screenrightsRequestDto in screenrightsRequestDtos)
            {
                if (screenrightsRequestDto.ScreenrightsRequestStatus == ScreenrightsRequestStatus.Processed)
                {
                    screenrightsRequestDto.Url = config["oscarstorage:blob"] +
                        ContainerName.SCREENRIGHTS +
                        Path.DirectorySeparatorChar +
                        screenrightsRequestDto.FileName;
                }
               
            }

            Logger.LogInformation((int)ScreenrightsRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<ScreenrightsRequestDto>.Create(screenrightsRequestDtos).WithTotal(total));
        }

        private IOrderedQueryable<Oscar.Core.Entities.ScreenrightsRequest> GetScreenrightsRequests(GetScreenrightsRequestsQuery request) =>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Oscar.Core.Entities.ScreenrightsRequest> BuildQuery(GetScreenrightsRequestsQuery request) =>
            OscarContext.ScreenrightsRequests
                .WhereDynamic(BuildPredicate(request));
        private Expression<Func<Oscar.Core.Entities.ScreenrightsRequest, string>> BuildOrderByExpression(GetScreenrightsRequestsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.WorksKnownColumns)}";

        private Expression<Func<Oscar.Core.Entities.ScreenrightsRequest, string>> BuildPredicate(GetScreenrightsRequestsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.WorksKnownColumns);
    }
}
