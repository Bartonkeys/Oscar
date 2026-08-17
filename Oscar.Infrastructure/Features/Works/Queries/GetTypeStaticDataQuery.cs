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
    public class GetTypeStaticDataQuery : BaseTableQuery, IRequest<Result<List<WorksTypeDto>>>
    {
        public int Id { get; set; }
    }

    public class GetTypeStaticDataQueryHandler : AbstractBaseHandler<GetTypeStaticDataQuery, List<WorksTypeDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetTypeStaticDataQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetTypeStaticDataQuery> validator, ILogger<GetTypeStaticDataQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<List<WorksTypeDto>>> HandleRequest(GetTypeStaticDataQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.WORK_TYPES);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var dataAsString = Encoding.UTF8.GetString(dataFromCache);
                    var dataAsJson = JsonSerializer.Deserialize<List<Core.Entities.WorksType>>(dataAsString);
                    Logger.LogInformation((int)WorksFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(dataAsJson.Select(a => Mapper.Map<WorksTypeDto>(a)).ToList());
                }
            }

            var type = OscarContext.WorksTypes.ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.WORK_TYPES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(type))); }

            Logger.LogInformation((int)WorksFeatureEvent.GetType, CommandResult.SUCCESS);

            return Result.Ok(type.Select(a => Mapper.Map<WorksTypeDto>(a)).ToList());
        }
    }
}
