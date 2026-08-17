using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BartonKeys.Extensions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Logging;
using Oscar.MRIT.Core.DTOs;
using Oscar.Mrit.Data;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    internal class WorksResolver : IValueResolver<FelixMritMatchDto, Match, ICollection<Works>>
    {
        private readonly FelixMritContext _felixMritContext;
        private readonly ILogger<WorksResolver> _logger;

        public WorksResolver(FelixMritContext felixMritContext, ILogger<WorksResolver> logger)
        {
            _felixMritContext = felixMritContext;
            _logger = logger;
        }

        public ICollection<Works> Resolve(FelixMritMatchDto source, Match destination, ICollection<Works> destMember,
            ResolutionContext context)
        {
            _logger.LogInformation($"Start resolving works");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var works = new List<Works>();

            foreach (var worksId in source.WorksIds)
            {
                _felixMritContext.Works.Cacheable().SingleOrDefault(c => c.WorksId == worksId).ToMaybe()
                    .Match(existingWorks => works.Add(existingWorks),
                        () => works.Add(new Works { WorksId = worksId }));
            }

            watch.Stop();
            _logger.LogInformation($"Processed works in {watch.ElapsedMilliseconds} milliseconds");

            return works;
        }
    }
}