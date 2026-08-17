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

namespace Oscar.Infrastructure.Features.Season.Queries
{
    public class GetSeasonBasicByIdQuery: BaseTableQuery, IRequest<Result<SeasonDto>>
    {
        public int Id { get; set; }
    }

    public class SeasonBasicByIdHandler : AbstractBaseHandler<GetSeasonBasicByIdQuery, SeasonDto>
    {
        public SeasonBasicByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSeasonBasicByIdQuery> validator, ILogger<GetSeasonBasicByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<SeasonDto>> HandleRequest(GetSeasonBasicByIdQuery request, CancellationToken cancellationToken)
        {

            var season = await OscarContext.Seasons
                .AsNoTracking()
                .Include(i => i.Titles)
                .Include(i => i.Episodes)!.ThenInclude(e => e.Titles)
                .Include(i => i.Series)!.ThenInclude(s => s.Titles)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            Logger.LogInformation((int)SeasonFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<SeasonDto>(season));
        }

    }
}
