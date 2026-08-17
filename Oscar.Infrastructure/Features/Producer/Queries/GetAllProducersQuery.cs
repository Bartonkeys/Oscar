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

namespace Oscar.Infrastructure.Features.Producer.Queries
{
    public class GetAllProducersQuery : IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllProducersHandler : AbstractBaseHandler<GetAllProducersQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllProducersHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllProducersQuery> validator, 
            ILogger<GetAllProducersQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;

        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllProducersQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var producersFromCache = await _cache.GetAsync(CacheKey.PRODUCERS);
                if ((producersFromCache?.Count() ?? 0) > 0)
                {
                    var producersString = Encoding.UTF8.GetString(producersFromCache);
                    var producersJson = JsonSerializer.Deserialize<List<Core.Entities.Producer>>(producersString);
                    Logger.LogInformation((int)ProducerFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(producersJson.Select(a => Mapper.Map<PersonDto>(a)));
                }
            }
            var producers = OscarContext.Producers.AsNoTracking().ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.PRODUCERS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(producers))); }

            Logger.LogInformation((int)ProducerFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(producers.Select(a => Mapper.Map<PersonDto>(a)));
        }

    }
}
