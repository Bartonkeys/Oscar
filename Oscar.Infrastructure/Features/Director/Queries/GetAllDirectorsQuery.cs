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
using System.Configuration;

namespace Oscar.Infrastructure.Features.Director.Queries
{
    public class GetAllDirectorsQuery: IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllDirectorsHandler : AbstractBaseHandler<GetAllDirectorsQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllDirectorsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllDirectorsQuery> validator, 
            ILogger<GetAllDirectorsQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllDirectorsQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var directorsFromCache = await _cache.GetAsync(CacheKey.DIRECTORS, cancellationToken);
                if ((directorsFromCache?.Count() ?? 0) > 0)
                {
                    var directorsJson = GetCachedDataDeserialize<PersonDto>(directorsFromCache);
                    if (directorsJson != null)
                    {
                        Logger.LogInformation((int)DirectorFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                        return Result.Ok(directorsJson);
                    }
                }
            }
            var directors = await OscarContext.Directors.AsNoTracking().Select(a => Mapper.Map<PersonDto>(a)).ToListAsync(cancellationToken);
            if (bool.Parse(_config["UseCache"]) && directors.Count > 0)
            {
                await _cache.SetAsync(CacheKey.DIRECTORS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(directors)));
            }

            Logger.LogInformation((int)DirectorFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(directors.AsEnumerable());
        }

    }
}
