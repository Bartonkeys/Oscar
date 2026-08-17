using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
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
    public class GetWorksSubTypeStaticDataQuery : BaseTableQuery, IRequest<Result<List<WorksSubTypeDto>>>
    {
        public int Id { get; set; }
    }

    public class GetWorksSubTypeStaticDataQueryHandler : AbstractBaseHandler<GetWorksSubTypeStaticDataQuery, List<WorksSubTypeDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetWorksSubTypeStaticDataQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksSubTypeStaticDataQuery> validator, ILogger<GetWorksSubTypeStaticDataQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<List<WorksSubTypeDto>>> HandleRequest(GetWorksSubTypeStaticDataQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.WORK_SUBTYPES);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsString = Encoding.UTF8.GetString(dataFromCache);
                    var dataAsJson = JsonSerializer.Deserialize<List<Core.Entities.WorksSubType>>(dataAsString);
                    Logger.LogInformation((int)WorksFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(dataAsJson.Select(a => Mapper.Map<WorksSubTypeDto>(a)).ToList());
                }
            }

            var worksSubType = OscarContext.WorksSubTypes.ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.WORK_SUBTYPES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(worksSubType))); }

            Logger.LogInformation((int)WorksFeatureEvent.GetWorksSubType, CommandResult.SUCCESS);

            return Result.Ok(worksSubType.Select(a => Mapper.Map<WorksSubTypeDto>(a)).ToList());
        }
    }
}
