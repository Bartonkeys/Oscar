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

namespace Oscar.Infrastructure.Features.ScreenWriter.Queries
{
    public class GetAllScreenWritersQuery : IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllScreenWritersHandler : AbstractBaseHandler<GetAllScreenWritersQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllScreenWritersHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllScreenWritersQuery> validator, 
            ILogger<GetAllScreenWritersQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;

        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllScreenWritersQuery request, CancellationToken cancellationToken)
        {

            if (bool.Parse(_config["UseCache"]) == true)
            {
                var screenwritersFromCache = await _cache.GetAsync(CacheKey.SCREENWRITERS);
                if ((screenwritersFromCache?.Count() ?? 0) > 0)
                {
                    var screenwritersString = Encoding.UTF8.GetString(screenwritersFromCache);
                    var screenwritersJson = JsonSerializer.Deserialize<List<Core.Entities.ScreenWriter>>(screenwritersString);
                    Logger.LogInformation((int)ScreenWriterFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(screenwritersJson.Select(a => Mapper.Map<PersonDto>(a)));
                }
            }
            var screenwriters = OscarContext.ScreenWriters.AsNoTracking().ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.SCREENWRITERS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(screenwriters))); }

            Logger.LogInformation((int)ScreenWriterFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(screenwriters.Select(a => Mapper.Map<PersonDto>(a)));

        }

    }
}
