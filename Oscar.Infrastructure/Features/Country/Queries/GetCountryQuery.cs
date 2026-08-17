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
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Linq.Expressions;

namespace Oscar.Infrastructure.Features.Country.Queries
{
    public class GetCountryByIdQuery: BaseTableQuery, IRequest<Result<IEntityTable<CountryDto>>>
    {
        public int Id { get; set; }
    }

    public class GetCountryByIdQueryHandler : AbstractBaseHandler<GetCountryByIdQuery, IEntityTable<CountryDto>>
    {
        public GetCountryByIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCountryByIdQuery> validator, ILogger<GetCountryByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<CountryDto>>> HandleRequest(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var countries = await OscarContext.Country.Where(c => c.Id == request.Id).ToListAsync(cancellationToken);
            var total = countries.Count();
            Logger.LogInformation((int)CountryFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<CountryDto>.Create(countries.Select(c => Mapper.Map<CountryDto>(c))).WithTotal(total));
        }
    }

    public class GetCountryQuery : BaseTableQuery, IRequest<Result<IEntityTable<CountryDto>>>
    {
        public GetCountryQuery()
        {
            BaseEntityName = "Country";
            SortColumn = "Name";
            SortDirection = "ascending";
        }
    }

    public class CountriesHandler : AbstractBaseHandler<GetCountryQuery, IEntityTable<CountryDto>>
    {
        private readonly IDynamicExpressionBuilderService _dynamicExpressionBuilderService;

        public CountriesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetCountryQuery> validator, ILogger<GetCountryQuery> logger, IDynamicExpressionBuilderService dynamicExpressionBuilderService) : base(oscarContext, mapper, validator, logger)
        {
            _dynamicExpressionBuilderService = dynamicExpressionBuilderService;
        }

        protected override async Task<Result<IEntityTable<CountryDto>>> HandleRequest(GetCountryQuery request, CancellationToken cancellationToken)
        {
            var items = await GetCountries(request).ToListAsync(cancellationToken);
            var total = items.Count();
            var pagedItems = items.Skip(request.Start).Take(request.Take);

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(EntityTable<CountryDto>.Create(pagedItems.Select(c => Mapper.Map<CountryDto>(c))).WithTotal(total));
        }

        private IOrderedQueryable<Core.Entities.Country> GetCountries(GetCountryQuery request) => request.SortDirection.Equals("descending", StringComparison.OrdinalIgnoreCase)
                ? BuildQuery(request).OrderByDescendingDynamic(BuildOrderByExpression(request))
                : BuildQuery(request).OrderByDynamic(BuildOrderByExpression(request));

        private IQueryable<Core.Entities.Country> BuildQuery(GetCountryQuery request) => OscarContext.Countries
                .WhereDynamic(BuildPredicate(request));

        private Expression<Func<Core.Entities.Country, string>> BuildOrderByExpression(GetCountryQuery request) =>
            c => $"c.{_dynamicExpressionBuilderService.ReplaceKnownColumnNames(request.SortColumn, Dictionaries.CountryKnownColumns)}";

        private Expression<Func<Core.Entities.Country, string>> BuildPredicate(GetCountryQuery request) =>
            c => _dynamicExpressionBuilderService.GenerateDynamicWhereExpression(request, Dictionaries.CountryKnownColumns);
    }
}
