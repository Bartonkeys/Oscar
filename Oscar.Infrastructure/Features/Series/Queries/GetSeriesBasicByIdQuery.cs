using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Queries
{
    public class GetSeriesBasicByIdQuery: BaseTableQuery, IRequest<Result<SeriesDto>>
    {
        public int Id { get; set; }
    }

    public class SeriesBasicByIdHandler : AbstractBaseHandler<GetSeriesBasicByIdQuery, SeriesDto>
    {
        public SeriesBasicByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSeriesBasicByIdQuery> validator, ILogger<GetSeriesBasicByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<SeriesDto>> HandleRequest(GetSeriesBasicByIdQuery request, CancellationToken cancellationToken)
        {

            var series = await OscarContext.Series
                .AsNoTracking()
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            Logger.LogInformation((int)SeriesFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<SeriesDto>(series));
        }
    }
}
