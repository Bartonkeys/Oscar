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


namespace Oscar.Infrastructure.Features.Matching.Queries
{
    public class GetMatchRequestsQuery : BaseTableQuery, IRequest<Result<IEntityTable<MatchRequestDto>>>
    {
        public GetMatchRequestsQuery()
        {
            BaseEntityName = "MatchRequest";
        }
    }

    public class MatchRequestsQueryHandler : AbstractBaseHandler<GetMatchRequestsQuery, IEntityTable<MatchRequestDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        private readonly IConfiguration config;


        public MatchRequestsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetMatchRequestsQuery> validator, ILogger<GetMatchRequestsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
            config = configuration;

        }

        protected override async Task<Result<IEntityTable<MatchRequestDto>>> HandleRequest(GetMatchRequestsQuery request, CancellationToken cancellationToken)
        {
            var matchRequests = GetMatchRequests(request);
            var total = matchRequests.Count();
            var pagedMatchRequests = matchRequests
                .Skip(request.Start)
                .Take(request.Take);


            var matchRequestDtos = pagedMatchRequests.Select(c => Mapper.Map<MatchRequestDto>(c)).ToList();
            foreach (var matchRequestDto in matchRequestDtos)
            {
                if (matchRequestDto.Status == MatchRequestStatus.Success)
                {
                    matchRequestDto.MatchingResultPublicUrl = config["oscarstorage:blob"] + ContainerName.MATCH + Path.DirectorySeparatorChar + matchRequestDto.Reference + "_MATCHED.csv";
                }
               
            }

            Logger.LogInformation((int)MatchRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<MatchRequestDto>.Create(matchRequestDtos).WithTotal(total));
        }

        private IOrderedQueryable<Oscar.Core.Entities.MatchRequest> GetMatchRequests(GetMatchRequestsQuery request) =>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Oscar.Core.Entities.MatchRequest> BuildQuery(GetMatchRequestsQuery request) =>
            OscarContext.MatchRequests
                .WhereDynamic(BuildPredicate(request));
        private Expression<Func<Oscar.Core.Entities.MatchRequest, string>> BuildOrderByExpression(GetMatchRequestsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.WorksKnownColumns)}";

        private Expression<Func<Oscar.Core.Entities.MatchRequest, string>> BuildPredicate(GetMatchRequestsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.WorksKnownColumns);
    }
}
