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
    public class SeriesSearchByTitleQuery: BaseTableQuery, IRequest<Result<IEntityTable<WorksDto>>>
    {
        public string Title { get; set; }
    }

    public class SeriesSearchByTitleQueryHandler : AbstractBaseHandler<SeriesSearchByTitleQuery, IEntityTable<WorksDto>>
    {
        public SeriesSearchByTitleQueryHandler(OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<SeriesSearchByTitleQuery> validator, 
            ILogger<SeriesSearchByTitleQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<WorksDto>>> HandleRequest(SeriesSearchByTitleQuery request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series
                .AsNoTracking()
                .Include(t => t.Titles)
                .Include(w => w.Actors)
                .Include(w => w.Directors)
                .Include(w => w.Producers)
                .Include(w => w.ScreenWriters)
                .Where(s => s.Titles.Any(t => EF.Functions.Like(t.Title!, request.Title +"%") 
                                              || EF.Functions.Like(t.ReverseTitle!, Reverse(request.Title) + "%")));

            var total = series.Count();
            var results = series.Skip(request.Start).Take(request.Take);

            return Result.Ok(EntityTable<WorksDto>.Create(results.Select(c => Mapper.Map<WorksDto>(c))).WithTotal(total));
        }

        public static string Reverse(string input)
        {
            return string.Create(input.Length, input, (chars, state) =>
            {
                state.AsSpan().CopyTo(chars);
                chars.Reverse();
            });
        }
    }
}
