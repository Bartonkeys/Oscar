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

namespace Oscar.Infrastructure.Features.Works.Queries
{
    public class GetLanguageStaticDataQuery : BaseTableQuery, IRequest<Result<List<LanguageDto>>>
    {
        public int Id { get; set; }
    }

    public class GetLanguageStaticDataQueryHandler : AbstractBaseHandler<GetLanguageStaticDataQuery, List<LanguageDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetLanguageStaticDataQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetLanguageStaticDataQuery> validator, ILogger<GetLanguageStaticDataQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<List<LanguageDto>>> HandleRequest(GetLanguageStaticDataQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]))
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.LANGUAGES, cancellationToken);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsJson = GetCachedDataDeserialize<LanguageDto>(dataFromCache);
                    Logger.LogInformation((int)WorksFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    if (dataAsJson != null)
                        return Result.Ok(dataAsJson.ToList());
                }
            }

            var language = OscarContext.Languages.AsNoTracking().Select(a => Mapper.Map<LanguageDto>(a)).ToList();

            if (bool.Parse(_config["UseCache"]) && language.Count > 0)
            {
                await _cache.SetAsync(CacheKey.LANGUAGES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(language)));
            }

            Logger.LogInformation((int)WorksFeatureEvent.GetLanguage, CommandResult.SUCCESS);
            return Result.Ok(language);
        }
    }
}
