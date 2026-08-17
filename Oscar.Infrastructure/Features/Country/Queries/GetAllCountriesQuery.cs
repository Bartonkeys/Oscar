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
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Oscar.Infrastructure.Features.Country.Queries
{
    public class GetAllCountriesQuery: IRequest<Result<IEnumerable<CountryDto>>>
    {
    }
    
    public class GetAllCountriesHandler : AbstractBaseHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllCountriesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllCountriesQuery> validator, 
            ILogger<GetAllCountriesQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;

        }

        protected override async Task<Result<IEnumerable<CountryDto>>> HandleRequest(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {

            if (bool.Parse(_config["UseCache"]) == true) {
                var countriesFromCache = await _cache.GetAsync(CacheKey.COUNTRIES);
                if ((countriesFromCache?.Count() ?? 0) > 0)
                {
                    var countriesString = Encoding.UTF8.GetString(countriesFromCache);
                    var countriesJson = JsonSerializer.Deserialize<List<Core.Entities.Country>>(countriesString);
                    Logger.LogInformation((int)CountryFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(countriesJson.Select(a => Mapper.Map<CountryDto>(a)));
                }
            }
            var countries = OscarContext.Countries.AsNoTracking().ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.COUNTRIES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(countries))); }

            Logger.LogInformation((int)CountryFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(countries.Select(a => Mapper.Map<CountryDto>(a)));
        }

    }
}
