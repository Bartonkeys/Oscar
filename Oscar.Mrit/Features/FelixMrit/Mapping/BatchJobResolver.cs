using System.Linq;
using AutoMapper;
using BartonKeys.Extensions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Logging;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Data;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    internal class BatchJobResolver : IValueResolver<FelixMritMatchDto, Match, BatchJob>
    {
        private readonly FelixMritContext _felixMritContext;
        private readonly ILogger<BatchJobResolver> _logger;

        public BatchJobResolver(FelixMritContext felixMritContext, ILogger<BatchJobResolver> logger)
        {
            _felixMritContext = felixMritContext;
            _logger = logger;
        }

        public BatchJob Resolve(FelixMritMatchDto source, Match destination, BatchJob destMember,
            ResolutionContext context)
        {
            _logger.LogInformation($"Start resolving batch job");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var batchJob = new BatchJob();

            _felixMritContext.BatchJobs.Cacheable()
                .SingleOrDefault(b => b.BatchJobKey == source.BatchJobKey)
                .ToMaybe()
                .Match(b => batchJob = b, () => batchJob.BatchJobKey = source.BatchJobKey);

            watch.Stop();
            _logger.LogInformation($"Processed batch job in {watch.ElapsedMilliseconds} milliseconds");

            return batchJob;
        }
    }
}