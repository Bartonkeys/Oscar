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


namespace Oscar.Infrastructure.Features.Equivalence.Queries
{
    public class GetEquivalenceRequestsQuery : BaseTableQuery, IRequest<Result<IEntityTable<EquivalenceRequestDto>>>
    {
        public GetEquivalenceRequestsQuery()
        {
            BaseEntityName = "EquivalenceRequest";
        }
    }

    public class EquivalenceRequestsQueryHandler : AbstractBaseHandler<GetEquivalenceRequestsQuery, IEntityTable<EquivalenceRequestDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        private readonly IConfiguration config;


        public EquivalenceRequestsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetEquivalenceRequestsQuery> validator, ILogger<GetEquivalenceRequestsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
            config = configuration;

        }

        protected override async Task<Result<IEntityTable<EquivalenceRequestDto>>> HandleRequest(GetEquivalenceRequestsQuery request, CancellationToken cancellationToken)
        {
            var equivalenceRequests = GetEquivalenceRequests(request);
            var total = equivalenceRequests.Count();
            var pagedEquivalenceRequests = equivalenceRequests
                .Skip(request.Start)
                .Take(request.Take);


            var equivalenceRequestDtos = pagedEquivalenceRequests.Select(c => Mapper.Map<EquivalenceRequestDto>(c)).ToList();
            foreach (var equivalenceRequestDto in equivalenceRequestDtos)
            {
                if (equivalenceRequestDto.EquivalenceRequestStatus == EquivalenceRequestStatus.Processed)
                {
                    equivalenceRequestDto.Url ??= config["oscarstorage:blob"] +
                        ContainerName.EQUIVALENCE +
                        Path.DirectorySeparatorChar +
                        equivalenceRequestDto.FileName;
                }

            }

            Logger.LogInformation((int)EquivalenceRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<EquivalenceRequestDto>.Create(equivalenceRequestDtos).WithTotal(total));
        }

        private IOrderedQueryable<Oscar.Core.Entities.EquivalenceRequest> GetEquivalenceRequests(GetEquivalenceRequestsQuery request) =>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Oscar.Core.Entities.EquivalenceRequest> BuildQuery(GetEquivalenceRequestsQuery request) =>
            OscarContext.EquivalenceRequests
                .WhereDynamic(BuildPredicate(request));
        private Expression<Func<Oscar.Core.Entities.EquivalenceRequest, string>> BuildOrderByExpression(GetEquivalenceRequestsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.WorksKnownColumns)}";

        private Expression<Func<Oscar.Core.Entities.EquivalenceRequest, string>> BuildPredicate(GetEquivalenceRequestsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.WorksKnownColumns);
    }
}
