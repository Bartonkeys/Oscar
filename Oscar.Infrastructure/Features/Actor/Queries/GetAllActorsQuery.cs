using System.Text;
using System.Text.Json;
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

namespace Oscar.Infrastructure.Features.Actor.Queries
{
    public class GetAllActorsQuery: IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllActorsHandler : AbstractBaseHandler<GetAllActorsQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllActorsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllActorsQuery> validator, 
            ILogger<GetAllActorsQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger )
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllActorsQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]))
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.ACTORS, cancellationToken);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsJson = GetCachedDataDeserialize<PersonDto>(dataFromCache);
                    Logger.LogInformation((int)ActorFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    if (dataAsJson != null) 
                        return Result.Ok(dataAsJson);
                }
            }

            var actors = OscarContext.Actors.AsNoTracking().Select(a => Mapper.Map<PersonDto>(a)).ToList();

            if (bool.Parse(_config["UseCache"]) && actors.Count > 0)
            {
                await _cache.SetAsync(CacheKey.ACTORS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(actors)));
            }
            Logger.LogInformation((int)ActorFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(actors.AsEnumerable());
        }
    }
}
