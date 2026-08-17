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

namespace Oscar.Infrastructure.Features.Clients.Queries
{
    public class GetClientsQuery: BaseTableQuery, IRequest<Result<IEntityTable<ClientDto>>>
    {
        public GetClientsQuery()
        {
            BaseEntityName = "Clients";
        }
    }

    public class ClientsHandler : AbstractBaseHandler<GetClientsQuery, IEntityTable<ClientDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;

        public ClientsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetClientsQuery> validator, ILogger<GetClientsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
        }

        protected override async Task<Result<IEntityTable<ClientDto>>> HandleRequest(GetClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await GetClients(request).ToListAsync(cancellationToken);
            var total = clients.Count();
            var pagedClients = clients
                .Skip(request.Start)
                .Take(request.Take);

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(EntityTable<ClientDto>.Create(pagedClients.Select(c => Mapper.Map<ClientDto>(c))).WithTotal(total));
        }

        private IOrderedQueryable<Client> GetClients(GetClientsQuery request) => request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Client> BuildQuery(GetClientsQuery request) => OscarContext.Clients
                .WhereDynamic(BuildPredicate(request));

        private Expression<Func<Client, string>> BuildOrderByExpression(GetClientsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.ClientKnownColumns)}";

        private Expression<Func<Client, string>> BuildPredicate(GetClientsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.ClientKnownColumns);
    }
}
