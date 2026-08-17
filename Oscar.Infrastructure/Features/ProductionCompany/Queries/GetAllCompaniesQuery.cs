using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Text;
using System.Text.Json;

namespace Oscar.Infrastructure.Features.ProductionCompany.Queries
{
    public class GetAllCompaniesQuery: IRequest<Result<IEnumerable<CompanyDto>>>
    {
    }
    
    public class GetAllCompaniesHandler : AbstractBaseHandler<GetAllCompaniesQuery, IEnumerable<CompanyDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllCompaniesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllCompaniesQuery> validator, 
            ILogger<GetAllCompaniesQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<CompanyDto>>> HandleRequest(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]))
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.COMPANIES, cancellationToken);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsJson = GetCachedDataDeserialize<CompanyDto>(dataFromCache);
                    Logger.LogInformation((int)CompanyFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    if (dataAsJson != null)
                        return Result.Ok(dataAsJson);
                }
            }

            var companies = OscarContext.Companies.AsNoTracking().Select(a => Mapper.Map<CompanyDto>(a)).ToList();

            if (bool.Parse(_config["UseCache"]) && companies.Count > 0)
            {
                await _cache.SetAsync(CacheKey.COMPANIES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(companies)));
            }

            Logger.LogInformation((int)CompanyFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(companies.AsEnumerable());
        }

    }

    
}
