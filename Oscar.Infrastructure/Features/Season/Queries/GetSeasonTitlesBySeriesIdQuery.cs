using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Season.Queries
{
    public class GetSeasonTitlesBySeriesIdQuery : BaseTableQuery, IRequest<Result<IEnumerable<WorksTitleDto>>>
    {
        public int SeriesId { get; set; }
    }

    public class SeasonSearchBySeriesIdQueryHandler : AbstractBaseHandler<GetSeasonTitlesBySeriesIdQuery, IEnumerable<WorksTitleDto>>
    {
        public SeasonSearchBySeriesIdQueryHandler(OscarContext oscarContext,
            IMapper mapper,
            IValidator<GetSeasonTitlesBySeriesIdQuery> validator,
            ILogger<GetSeasonTitlesBySeriesIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<WorksTitleDto>>> HandleRequest(GetSeasonTitlesBySeriesIdQuery request, CancellationToken cancellationToken)
        {
            var Season = OscarContext.Seasons
                .AsNoTracking()
                .Include(t => t.Titles)
                .Where(s => s.SeriesId == request.SeriesId);

            var result = Season.ToList().Select(s => new WorksTitleDto
            {
                Id = s.Id,
                Title = s.Titles.First().Title
            });
            return Result.Ok(result);
        }
    }
}
