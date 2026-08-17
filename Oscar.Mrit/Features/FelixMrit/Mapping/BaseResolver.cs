using AutoMapper;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Logging;
using Oscar.Mrit.Data;
using System.Collections.Generic;
using System.Linq;
using BartonKeys.Extensions;
using Oscar.MRIT.Core.DTOs;

namespace Oscar.Mrit.Features.FelixMrit.Mapping
{
    internal abstract class BaseResolver<T, TS, TD> : IValueResolver<TS, TD, ICollection<T>>
        where T : BaseName, new()
    {
        private readonly FelixMritContext _felixMritContext;
        private readonly ILogger<BaseResolver<T, TS, TD>> _logger;

        protected abstract IList<string> GetNames(TS source);

        protected BaseResolver(FelixMritContext felixMritContext, ILogger<BaseResolver<T, TS, TD>> logger)
        {
            _felixMritContext = felixMritContext;
            _logger = logger;
        }

        public ICollection<T> Resolve(TS source, TD destination, ICollection<T> destMember,
            ResolutionContext context)
        {
            var entities = _felixMritContext.Set<T>();
            var resolvedNames = new List<T>();

            _logger.LogInformation($"Start resolving {typeof(T)}");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var name in GetNames(source))
            {
                entities.Cacheable().SingleOrDefault(c => c.Name == name).ToMaybe()
                    .Match(existingEntity => resolvedNames.Add(existingEntity),
                        () =>
                        {
                            var newEntity = new T { Name = name };
                            entities.Add(newEntity);
                            _felixMritContext.SaveChanges();
                            resolvedNames.Add(newEntity);
                        });
            }

            watch.Stop();
            _logger.LogInformation($"Processed  {typeof(T)} in {watch.ElapsedMilliseconds} milliseconds");

            return resolvedNames;
        }
    }

    internal class CompanyResolver : BaseResolver<Company, FelixMritMatchDto, Match>
    {
        public CompanyResolver(FelixMritContext felixMritContext, ILogger<CompanyResolver> logger) : base(felixMritContext, logger)
        { }

        protected override IList<string> GetNames(FelixMritMatchDto source) => source.Companies;
    }

    internal class CountryResolver : BaseResolver<Country, FelixMritMatchDto, Match>
    {
        public CountryResolver(FelixMritContext felixMritContext, ILogger<CountryResolver> logger) : base(felixMritContext, logger)
        { }
        protected override IList<string> GetNames(FelixMritMatchDto source) => source.Countries;
    }

    internal class GenreResolver : BaseResolver<Genre, FelixMritMatchDto, Match>
    {
        public GenreResolver(FelixMritContext felixMritContext, ILogger<GenreResolver> logger) : base(felixMritContext, logger)
        { }

        protected override IList<string> GetNames(FelixMritMatchDto source) => source.Genres;
    }

    internal class TerritoryResolver : BaseResolver<Territory, TransmissionDto, Transmission>
    {
        public TerritoryResolver(FelixMritContext felixMritContext, ILogger<TerritoryResolver> logger) : base(felixMritContext, logger)
        { }

        protected override IList<string> GetNames(TransmissionDto source) => source.Territories;
    }

    internal class AltProductionTitleResolver : BaseResolver<AltProductionTitle, FelixMritMatchDto, Match>
    {
        public AltProductionTitleResolver(FelixMritContext felixMritContext, ILogger<AltProductionTitleResolver> logger) : base(felixMritContext, logger)
        { }

        protected override IList<string> GetNames(FelixMritMatchDto source) => source.AltProductionTitles;
    }

    internal class AltRecordTitleResolver : BaseResolver<AltRecordTitle, FelixMritMatchDto, Match>
    {
        public AltRecordTitleResolver(FelixMritContext felixMritContext, ILogger<AltRecordTitleResolver> logger) : base(felixMritContext, logger)
        { }

        protected override IList<string> GetNames(FelixMritMatchDto source) => source.AltRecordTitles;
    }
}