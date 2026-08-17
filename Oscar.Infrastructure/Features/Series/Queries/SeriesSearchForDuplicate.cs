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

namespace Oscar.Infrastructure.Features.Series.Queries
{
    public class SeriesSearchForDuplicate : BaseTableQuery, IRequest<Result<List<DuplicateDto>>>
    {
        public String Title { get; set; }
        public int? ProductionYear { get; set; }
        public int? GenreId { get; set; }
        public int? DurationMinutes { get; set; }
        public ICollection<int> CountryIds { get; set; }
    }

    public class SeriesDuplicateSearchHandler : AbstractBaseHandler<SeriesSearchForDuplicate, List<DuplicateDto>>
    {
        public SeriesDuplicateSearchHandler(OscarContext oscarContext, IMapper mapper, IValidator<SeriesSearchForDuplicate> validator, ILogger<SeriesSearchForDuplicate> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<DuplicateDto>>> HandleRequest(SeriesSearchForDuplicate request, CancellationToken cancellationToken)
        {

            var series = await OscarContext.Series
                .AsNoTracking()
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .Include(i => i.Titles)
                .AsSplitQuery()
                .Where(s =>
                    s.Titles.Any(t => t.TitleType == TitleType.Main && t.Title == request.Title) &&
                    s.ProductionYear == request.ProductionYear &&
                    s.GenreId == request.GenreId &&
                    s.DurationMinutes == request.DurationMinutes &&
                    s.Countries.Any(c => request.CountryIds.Any(cc => cc == c.Id))
                ).ToListAsync();

            Logger.LogInformation((int)SeriesFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<List<DuplicateDto>>(series));
        }
    }
}
