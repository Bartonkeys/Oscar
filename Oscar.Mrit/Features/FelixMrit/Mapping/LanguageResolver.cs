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
    internal class LanguageResolver : IValueResolver<FelixMritMatchDto, Match, ICollection<Language>>
    {
        private readonly FelixMritContext _felixMritContext;
        private readonly ILogger<LanguageResolver> _logger;

        public LanguageResolver(FelixMritContext felixMritContext, ILogger<LanguageResolver> logger)
        {
            _felixMritContext = felixMritContext;
            _logger = logger;
        }

        public ICollection<Language> Resolve(FelixMritMatchDto source, Match destination, ICollection<Language> destMember,
            ResolutionContext context)
        {
            _logger.LogInformation($"Start resolving languages");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var languages = new List<Language>();

            foreach (var language in source.Languages)
            {
                _felixMritContext.Languages.Cacheable().SingleOrDefault(c => c.EnglishName == language).ToMaybe()
                    .Match(existingLanguage => languages.Add(existingLanguage),
                        () => languages.Add(new Language() { EnglishName = language }));
            }

            watch.Stop();
            _logger.LogInformation($"Processed languages in {watch.ElapsedMilliseconds} milliseconds");

            return languages;
        }
    }
}