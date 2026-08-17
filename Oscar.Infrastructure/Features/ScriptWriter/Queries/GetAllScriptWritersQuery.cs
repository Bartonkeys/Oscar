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
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Oscar.Infrastructure.Features.ScriptWriter.Queries
{
    public class GetAllScriptWritersQuery : IRequest<Result<IEnumerable<PersonDto>>>
    {
    }
    
    public class GetAllScriptWritersHandler : AbstractBaseHandler<GetAllScriptWritersQuery, IEnumerable<PersonDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllScriptWritersHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllScriptWritersQuery> validator, 
            ILogger<GetAllScriptWritersQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;

        }

        protected override async Task<Result<IEnumerable<PersonDto>>> HandleRequest(GetAllScriptWritersQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]) == true)
            {
                var scriptwritersFromCache = await _cache.GetAsync(CacheKey.SCRIPTWRITERS);
                if ((scriptwritersFromCache?.Count() ?? 0) > 0)
                {
                    var scriptwritersString = Encoding.UTF8.GetString(scriptwritersFromCache);
                    var scriptwritersJson = JsonSerializer.Deserialize<List<Core.Entities.ScriptWriter>>(scriptwritersString);
                    Logger.LogInformation((int)ScriptWriterFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                    return Result.Ok(scriptwritersJson.Select(a => Mapper.Map<PersonDto>(a)));
                }
            }
            var scriptwriters = OscarContext.ScriptWriters.AsNoTracking().ToList();

            if (bool.Parse(_config["UseCache"]) == true)
            { await _cache.SetAsync(CacheKey.SCRIPTWRITERS, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(scriptwriters))); }

            Logger.LogInformation((int)ScriptWriterFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(scriptwriters.Select(a => Mapper.Map<PersonDto>(a)));

        }

    }
}
