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
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetGenreSubTypeStaticDataQuery : BaseTableQuery, IRequest<Result<List<GenreSubTypeDto>>>
    {
        public int Id { get; set; }
    }

    public class GetGenreSubTypeStaticDataQueryHandler : AbstractBaseHandler<GetGenreSubTypeStaticDataQuery, List<GenreSubTypeDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetGenreSubTypeStaticDataQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetGenreSubTypeStaticDataQuery> validator, ILogger<GetGenreSubTypeStaticDataQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<List<GenreSubTypeDto>>> HandleRequest(GetGenreSubTypeStaticDataQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.GENRE_SUBTYPES);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsString = Encoding.UTF8.GetString(dataFromCache);
                    var dataAsJson = JsonSerializer.Deserialize<List<Core.Entities.GenreSubType>>(dataAsString);
                    Logger.LogInformation((int)WorksFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(dataAsJson.Select(a => Mapper.Map<GenreSubTypeDto>(a)).ToList());
                }
            }

            var genreSubType = OscarContext.GenreSubTypes.ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.GENRE_SUBTYPES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(genreSubType))); }

            Logger.LogInformation((int)WorksFeatureEvent.GetGenreSubType, CommandResult.SUCCESS);

            return Result.Ok(genreSubType.Select(a => Mapper.Map<GenreSubTypeDto>(a)).ToList());
        }
    }
}
