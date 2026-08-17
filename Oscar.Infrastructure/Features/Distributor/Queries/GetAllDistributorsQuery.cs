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
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Oscar.Infrastructure.Features.Distributor.Queries
{
    public class GetAllDistributorsQuery : IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllDistributorsHandler : AbstractBaseHandler<GetAllDistributorsQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllDistributorsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllDistributorsQuery> validator, 
            ILogger<GetAllDistributorsQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllDistributorsQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {

                var distributorsFromCache = await _cache.GetAsync(CacheKey.DISTRIBUTORS);
                if ((distributorsFromCache?.Count() ?? 0) > 0)
                {
                    var distributorsJson = GetCachedDataDeserialize<PersonDto>(distributorsFromCache);
                    Logger.LogInformation((int)DistributorFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    if (distributorsJson != null)
                        return Result.Ok(distributorsJson);
                }
            }
            var distributors = OscarContext.Distributors.AsNoTracking().Select(c=> Mapper.Map<PersonDto>(c)).ToList();

            if (bool.Parse(_config["UseCache"]) && distributors.Count > 0)
            {
                await _cache.SetAsync(CacheKey.DISTRIBUTORS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(distributors)));
            }

            Logger.LogInformation((int)DistributorFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(distributors.AsEnumerable());
        }

    }
}
