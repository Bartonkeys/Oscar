using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Common.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private ILogger<QueueService> _logger;
        private const int DEFAULT_CACHE_ABSOLUTE_EXPIRY_HOURS = 24;
        private const int DEFAULT_CACHE_SLIDING_EXPIRY_HOURS = 24;


        public CacheService(IDistributedCache cache, ILogger<QueueService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return _cache.GetAsync(key, token);
        }


        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions? options, CancellationToken token = default)
        {
            if (options is null)
            {
                options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(DEFAULT_CACHE_ABSOLUTE_EXPIRY_HOURS),
                    SlidingExpiration = TimeSpan.FromHours(DEFAULT_CACHE_SLIDING_EXPIRY_HOURS)
                };
            }
            await _cache.SetAsync(key, value, options);
        }

        public bool InvalidateCacheForEntity(BaseEntity cacheEntity)
        {
            switch (cacheEntity)
            {
                case Core.Entities.Client x:
                    _cache.RemoveAsync(CacheKey.CLIENTS);
                    return true;
                case Core.Entities.Company x:
                    _cache.RemoveAsync(CacheKey.COMPANIES);
                    return true;
                case Core.Entities.Country x:
                    _cache.RemoveAsync(CacheKey.COUNTRIES);
                    return true;
                case Core.Entities.Actor x:
                    _cache.RemoveAsync(CacheKey.ACTORS);
                    return true;
                case Core.Entities.Producer x:
                    _cache.RemoveAsync(CacheKey.PRODUCERS);
                    return true;
                case Core.Entities.ScreenWriter x:
                    _cache.RemoveAsync(CacheKey.SCREENWRITERS);
                    return true;
                case Core.Entities.ScriptWriter x:
                    _cache.RemoveAsync(CacheKey.SCRIPTWRITERS);
                    return true;
                case Core.Entities.Director x:
                    _cache.RemoveAsync(CacheKey.DIRECTORS);
                    return true;
                case Core.Entities.Distributor x:
                    _cache.RemoveAsync(CacheKey.DISTRIBUTORS);
                    return true;
                default:
                    Console.WriteLine("Can't clear cache for unknown entity");
                    return false;
                case null:
                    return false;
            }
        }

        public void InvalidateCacheByKey(string key)
        {
            switch (key)
            {
                case CacheKey.ACTORS:
                case CacheKey.CLIENTS:
                case CacheKey.COMPANIES:
                case CacheKey.COUNTRIES:
                case CacheKey.COUNTRIES_GROUPS:
                case CacheKey.DIRECTORS:
                case CacheKey.DISTRIBUTORS:
                case CacheKey.GENRES:
                case CacheKey.GENRE_SUBTYPES:
                case CacheKey.LANGUAGES:
                case CacheKey.PRODUCERS:
                case CacheKey.SCREENWRITERS:
                case CacheKey.SCRIPTWRITERS:
                case CacheKey.MANDATE_TYPES:
                case CacheKey.WORK_TYPES:
                case CacheKey.WORK_SUBTYPES:
                    _cache.RemoveAsync(key); break;
            }
        }

        public void InvalidateCache()
        {
            _cache.RemoveAsync(CacheKey.ACTORS);
            _cache.RemoveAsync(CacheKey.CLIENTS);
            _cache.RemoveAsync(CacheKey.COMPANIES);
            _cache.RemoveAsync(CacheKey.COUNTRIES);
            _cache.RemoveAsync(CacheKey.COUNTRIES_GROUPS);
            _cache.RemoveAsync(CacheKey.DIRECTORS);
            _cache.RemoveAsync(CacheKey.DISTRIBUTORS);
            _cache.RemoveAsync(CacheKey.GENRES);
            _cache.RemoveAsync(CacheKey.GENRE_SUBTYPES);
            _cache.RemoveAsync(CacheKey.LANGUAGES);
            _cache.RemoveAsync(CacheKey.PRODUCERS);
            _cache.RemoveAsync(CacheKey.SCREENWRITERS);
            _cache.RemoveAsync(CacheKey.SCRIPTWRITERS);
            _cache.RemoveAsync(CacheKey.MANDATE_TYPES);
            _cache.RemoveAsync(CacheKey.WORK_TYPES);
            _cache.RemoveAsync(CacheKey.WORK_SUBTYPES);
        }
    }
}
