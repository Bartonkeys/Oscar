using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Queries
{
    public class GetSeriesTitlesByCatalogueIdQuery : BaseTableQuery, IRequest<Result<IEnumerable<WorksTitleDto>>>
    {
        public int CatalogueId { get; set; }
    }

    public class SeriesSearchByCatalogueIdQueryHandler : AbstractBaseHandler<GetSeriesTitlesByCatalogueIdQuery, IEnumerable<WorksTitleDto>>
    {
        public SeriesSearchByCatalogueIdQueryHandler(OscarContext oscarContext,
            IMapper mapper,
            IValidator<GetSeriesTitlesByCatalogueIdQuery> validator,
            ILogger<GetSeriesTitlesByCatalogueIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<WorksTitleDto>>> HandleRequest(GetSeriesTitlesByCatalogueIdQuery request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .AsNoTracking()
                .Include(t => t.Titles)
                .Where(s => s.Catalogues.Any(c => c.Id == request.CatalogueId));

            var result = series.ToList().Select(s => new WorksTitleDto
            {
                Id = s.Id,
                Title = s.Titles.First().Title
            }); ;
            return Result.Ok(result);
        }
    }
}
