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

namespace Oscar.Infrastructure.Features.MandateTypes.Queries
{
    public class GetAllMandateTypesQuery : IRequest<Result<IEnumerable<MandateTypeDto>>>
    {
    }

    public class GetAllMandateTypesHandler : AbstractBaseHandler<GetAllMandateTypesQuery, IEnumerable<MandateTypeDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllMandateTypesHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllMandateTypesQuery> validator,
            ILogger<GetAllMandateTypesQuery> logger, IConfiguration configuration, ICacheService cache)
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<MandateTypeDto>>> HandleRequest(GetAllMandateTypesQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var mandateTypesFromCache = await _cache.GetAsync(CacheKey.MANDATE_TYPES);
                if ((mandateTypesFromCache?.Count() ?? 0) > 0)
                {
                    var mandateTypesString = Encoding.UTF8.GetString(mandateTypesFromCache);
                    var mandateTypesJson = JsonSerializer.Deserialize<List<Core.Entities.MandateType>>(mandateTypesString);
                    Logger.LogInformation((int)MandateTypeFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(mandateTypesJson.Select(a => Mapper.Map<MandateTypeDto>(a)));
                }
            }
            var mandateTypes = OscarContext.MandateType.AsNoTracking().OrderBy(x =>x.Name).ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.MANDATE_TYPES, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mandateTypes))); }

            Logger.LogInformation((int)MandateTypeFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(mandateTypes.Select(a => Mapper.Map<MandateTypeDto>(a)));
        }

    }
}
