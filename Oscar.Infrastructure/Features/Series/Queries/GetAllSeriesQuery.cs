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
    public class GetAllSeriesQuery : BaseTableQuery, IRequest<Result<IQueryable<LightWeightWorksDto>>>
    {
        public string Title { get; set; }
    }

    public class GetAllSeriesQueryHandler : AbstractBaseHandler<GetAllSeriesQuery, IQueryable<LightWeightWorksDto>>
    {
        public GetAllSeriesQueryHandler(OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<GetAllSeriesQuery> validator, 
            ILogger<GetAllSeriesQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IQueryable<LightWeightWorksDto>>> HandleRequest(GetAllSeriesQuery request, CancellationToken cancellationToken)
        {
            var series = OscarContext.Series.Select(s => new LightWeightWorksDto
            {
                Id = s.Id,
                Title = s.Titles.First().Title
            });

            return Result.Ok(series);
        }
    }

    public record LightWeightWorksDto 
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ReverseTitle { get; set; }
    }
}
