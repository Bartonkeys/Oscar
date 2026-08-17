using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Oscar.Infrastructure.Features.Country.Queries
{
    public class GetAllCountriesGroupsQuery: IRequest<Result<HashSet<CountriesGroupsDto>>>
    {
    }
    
    public class GetAllCountriesGroupsHandler : AbstractBaseHandler<GetAllCountriesGroupsQuery, HashSet<CountriesGroupsDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllCountriesGroupsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllCountriesGroupsQuery> validator, 
            ILogger<GetAllCountriesGroupsQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<HashSet<CountriesGroupsDto>>> HandleRequest(GetAllCountriesGroupsQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var countriesFromCache = await _cache.GetAsync(CacheKey.COUNTRIES_GROUPS);
                if ((countriesFromCache?.Count() ?? 0) > 0)
                {
                    var countriesGroupsDtoString = Encoding.UTF8.GetString(countriesFromCache);
                    var countriesGroupsDtoJson = JsonSerializer.Deserialize<HashSet<CountriesGroupsDto>>(countriesGroupsDtoString);
                    Logger.LogInformation((int)CountryGrouopFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(countriesGroupsDtoJson);
                }
            }

            var countriesGroupsDto = OscarContext.CountryGroup
                .Include(x => x.Countries)
                .Select(x => new CountriesGroupsDto
                {
                    Id = x.Id,
                    Code = x.GroupName,
                    Name = x.GroupName,
                    Children = x.Countries
                        .Select(c => new CountriesGroupsDto
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Name = c.Name,
                        }).ToHashSet()
                }).ToHashSet();

            if (bool.Parse(_config["UseCache"]) == true)
            { 
                await _cache.SetAsync(CacheKey.COUNTRIES_GROUPS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(countriesGroupsDto))); 
            }

            Logger.LogInformation((int)CountryGrouopFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(countriesGroupsDto);
        }
    }
}
