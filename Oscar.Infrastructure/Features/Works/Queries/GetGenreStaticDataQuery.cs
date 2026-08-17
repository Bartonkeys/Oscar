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
    public class GetGenreStaticDataQuery : BaseTableQuery, IRequest<Result<List<GenreDto>>>
    {
        public int Id { get; set; }
    }

    public class GetGenreStaticDataQueryHandler : AbstractBaseHandler<GetGenreStaticDataQuery, List<GenreDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetGenreStaticDataQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetGenreStaticDataQuery> validator, ILogger<GetGenreStaticDataQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<List<GenreDto>>> HandleRequest(GetGenreStaticDataQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.GENRES);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsString = Encoding.UTF8.GetString(dataFromCache);
                    var dataAsJson = JsonSerializer.Deserialize<List<Core.Entities.Genre>>(dataAsString);
                    Logger.LogInformation((int)WorksFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(dataAsJson.Select(a => Mapper.Map<GenreDto>(a)).ToList());
                }
            }

            var genre = OscarContext.Genres.ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.GENRES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(genre))); }

            Logger.LogInformation((int)WorksFeatureEvent.GetGenre, CommandResult.SUCCESS);

            return Result.Ok(genre.Select(a => Mapper.Map<GenreDto>(a)).ToList());
        }
    }
}
