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

namespace Oscar.Infrastructure.Features.Registration.Queries
{
    public class GetRegistrationsQuery : BaseTableQuery, IRequest<Result<IEntityTable<RegistrationDisplayDto>>>
    {
        public GetRegistrationsQuery()
        {
            BaseEntityName = "Registration";
        }
    }

    public class GetRegistrationsQueryHandler : AbstractBaseHandler<GetRegistrationsQuery, IEntityTable<RegistrationDisplayDto>>
    {
        IDynamicExpressionBuilderService DynamicExpressionBuilderService;
        private readonly IConfiguration config;

        public GetRegistrationsQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRegistrationsQuery> validator, ILogger<GetRegistrationsQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            DynamicExpressionBuilderService = dynamicExpressionBuilderService;
            config = configuration;
        }

        protected override async Task<Result<IEntityTable<RegistrationDisplayDto>>> HandleRequest(GetRegistrationsQuery request, CancellationToken cancellationToken)
        {
            var registrations = GetRegistrations(request).ToList();
  
            var total = registrations.Count();
            var pagedRegistrations = registrations
                .Skip(request.Start)
                .Take(request.Take);


            Logger.LogInformation((int)RegistrationFeatureEvent.Get, CommandResult.SUCCESS);

            var registrationDtos = pagedRegistrations.Select(c => Mapper.Map<RegistrationDisplayDto>(c)).ToList();
            foreach (var registration in registrationDtos)
            {
                Console.WriteLine(registration.SocietyName);
                registration.RegistrationBatch.RegistrationBatchResultPublicUrl = config["oscarstorage:blob"] + config["oscarstorage:container-registrations"] + Path.DirectorySeparatorChar + registration.RegistrationBatch.BatchId +
                    //(String.Equals(registration.SocietyName, "Agicoa", StringComparison.OrdinalIgnoreCase) ?  "_Agicoa.xml": "_REGISTERED.xml");
                    "_Agicoa.xml";
                registration.RegistrationBatch.RegistrationCount = registrations
                    .Where(x => x.RegistrationBatch!= null &&
                    x.RegistrationBatch.BatchId == registration.RegistrationBatch.BatchId) != null
                    ? registrations
                    .Where(x => x.RegistrationBatch != null &&
                    x.RegistrationBatch.BatchId == registration.RegistrationBatch.BatchId).Count()
                    : 0;
            }

            return Result.Ok(EntityTable<RegistrationDisplayDto>.Create(registrationDtos).WithTotal(total));
        }

        private IOrderedQueryable<Core.Entities.Registration> GetRegistrations(GetRegistrationsQuery request) =>
            request.SortDirection.Equals("descending")
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Core.Entities.Registration> BuildQuery(GetRegistrationsQuery request) =>
            OscarContext.Registrations
                .WhereDynamic(BuildPredicate(request))
                .Include(x => x.RegistrationBatch)
                .Include(x => x.Client)
                .Include(x => x.Works)
                .Include(x => x.Works.Titles)
                .Include(x => x.Society);

        private Expression<Func<Core.Entities.Registration, string>> BuildOrderByExpression(GetRegistrationsQuery request) =>
            c => $"c.{DynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.RegistrationKnownColumns)}";

        private Expression<Func<Core.Entities.Registration, string>> BuildPredicate(GetRegistrationsQuery request) =>
            c => DynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.RegistrationKnownColumns);

    }
}
