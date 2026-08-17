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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Oscar.Infrastructure.Features.Clients.Queries
{
    public class GetAllClientsQuery: IRequest<Result<IEnumerable< ClientDto>>>
    {
    }
    
    public class GetAllClientsHandler : AbstractBaseHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
    {
        private readonly IConfiguration _config;
        private readonly ICacheService _cache;

        public GetAllClientsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllClientsQuery> validator, 
            ILogger<GetAllClientsQuery> logger, IConfiguration configuration, ICacheService cache) 
            : base(oscarContext, mapper, validator, logger)
        {
            _config = configuration;
            _cache = cache;
        }

        protected override async Task<Result<IEnumerable<ClientDto>>> HandleRequest(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            if (bool.Parse(_config["UseCache"]))
            {
                var dataFromCache = await _cache.GetAsync(CacheKey.CLIENTS, cancellationToken);
                if ((dataFromCache?.Count() ?? 0) > 0)
                {
                    var clientsData = GetCachedDataDeserialize<ClientDto>(dataFromCache);
                    if (clientsData != null)
                    {
                        Logger.LogInformation((int)ClientFeatureEvent.GetFromCache, CommandResult.SUCCESS);
                        return Result.Ok(clientsData);
                    }
                }
            }

            Logger.LogInformation((int)ClientFeatureEvent.Get, "GET");
            var clients = await OscarContext.Clients
                .AsNoTracking()
                .Include(c => c.Catalogues)
                .Include(c => c.ClientAltNames)
                .Include(c => c.Contract)
                .Include(c => c.CustomerServiceManagers).ThenInclude(o => o.Operator)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var result = clients.Select(c => Mapper.Map<ClientDto>(c));
            if (bool.Parse(_config["UseCache"]) && clients.Count > 0)
            {
                await _cache.SetAsync(CacheKey.CLIENTS, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })),
                    new DistributedCacheEntryOptions { }, cancellationToken);
            }

            Logger.LogInformation((int)ClientFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(result.AsEnumerable());
        }
    }
}
