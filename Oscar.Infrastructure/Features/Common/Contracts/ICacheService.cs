using BartonKeys.Functional;
using Microsoft.Extensions.Caching.Distributed;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.Common.Contracts
{
    public interface ICacheService
    {
        Task<byte[]?> GetAsync(string key, CancellationToken token = default);
        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions? options = default, CancellationToken token = default);
        bool InvalidateCacheForEntity(BaseEntity cacheEntity);
        void InvalidateCacheByKey(string key);
        void InvalidateCache();
    }
}
