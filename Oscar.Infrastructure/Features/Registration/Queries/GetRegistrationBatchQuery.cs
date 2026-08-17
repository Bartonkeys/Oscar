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
using Microsoft.Extensions.Configuration;
using Oscar.Infrastructure.Features.Report.Queries;

namespace Oscar.Infrastructure.Features.Registration.Queries
{
    public class GetRegistrationBatchQuery : BaseTableQuery, IRequest<Result<IEntityTable<RegistrationBatchDisplayDto>>>
    {
        public GetRegistrationBatchQuery()
        {
            BaseEntityName = "RegistrationBatch";
        }
    }

    public class GetRegistrationBatchQueryQueryHandler : AbstractBaseHandler<GetRegistrationBatchQuery, IEntityTable<RegistrationBatchDisplayDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        private readonly IConfiguration config;

        public GetRegistrationBatchQueryQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRegistrationBatchQuery> validator, ILogger<GetRegistrationBatchQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
            config = configuration;
        }

        protected override async Task<Result<IEntityTable<RegistrationBatchDisplayDto>>> HandleRequest(GetRegistrationBatchQuery request, CancellationToken cancellationToken)
        {
            var registrationDateFrom = OscarContext.RegistrationConfigurations.Where(x => x.Name == request.BaseEntityName).FirstOrDefault().RegistrationDate;

            var registrationBatches = GetRegistrationBatch(request).Where(x => x.DateRegistered >= registrationDateFrom).ToList();
  
            var total = registrationBatches.Count();
            var pagedRegistrationBatches = registrationBatches
                .OrderByDescending(r => r.DateRegistered)
                .Skip(request.Start)
                .Take(request.Take);

            Logger.LogInformation((int)RegistrationFeatureEvent.Get, CommandResult.SUCCESS);

            var registrationBatchDtos = pagedRegistrationBatches.Select(c => Mapper.Map<RegistrationBatchDisplayDto>(c)).ToList();
            foreach (var registrationBatch in registrationBatchDtos)
            {
                //var registrationDtos = OscarContext.Registrations
                //        .Where(x => x.RegistrationBatch != null && x.RegistrationBatch.BatchId == registrationBatch.BatchId)
                //        .Include(X => X.Society);
                //registrationBatch.registrationDtos = Mapper.Map<List<RegistrationDisplayDto>>(registrationDtos);
                registrationBatch.RegistrationCount =
                    OscarContext.Registrations.Count(x => x.RegistrationBatch.BatchId == registrationBatch.BatchId);
                registrationBatch.RegistrationBatchResultPublicUrl = config["oscarstorage:blob"] + config["oscarstorage:container-registrations"] + Path.DirectorySeparatorChar + registrationBatch.FileName;
                //registrationBatch.RegistrationCount = registrationBatch != null && registrationBatch.registrationDtos != null ? registrationBatch.registrationDtos.Count() : 0;
                registrationBatch.ClientName = registrationBatch.IsAllClients ? "All" : OscarContext.Clients.SingleOrDefault(c => c.Id == registrationBatch.ClientId)?.ClientName;
                registrationBatch.CatalogueName = registrationBatch.CatalogueId != null ? OscarContext.Catalogues.SingleOrDefault(c => c.Id == registrationBatch.CatalogueId)?.Name : "All";
                registrationBatch.Society = registrationBatch.SocietyId != null && registrationBatch.SocietyId > 0 ? (await OscarContext.Societies.SingleAsync(s => s.Id == registrationBatch.SocietyId, cancellationToken: cancellationToken)).Name : "UNKNOWN";
            }

            return Result.Ok(EntityTable<RegistrationBatchDisplayDto>.Create(registrationBatchDtos).WithTotal(total));
        }

        private IOrderedQueryable<Core.Entities.RegistrationBatch> GetRegistrationBatch(GetRegistrationBatchQuery request) =>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Core.Entities.RegistrationBatch> BuildQuery(GetRegistrationBatchQuery request) =>
            OscarContext.RegistrationBatches.AsNoTracking()
                .WhereDynamic(BuildPredicate(request));

        private Expression<Func<Core.Entities.RegistrationBatch, string>> BuildOrderByExpression(GetRegistrationBatchQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.RegistrationKnownColumns)}";

        private Expression<Func<Core.Entities.RegistrationBatch, string>> BuildPredicate(GetRegistrationBatchQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.RegistrationKnownColumns);

    }
}
