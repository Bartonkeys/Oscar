using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Queries
{
    public class GetEpisodeBySeasonIdQuery: BaseTableQuery, IRequest<Result<List<EpisodeDto>>>
    {
        public int SeasonId { get; set; }
    }

    public class EpisodeBySeasonIdHandler : AbstractBaseHandler<GetEpisodeBySeasonIdQuery, List<EpisodeDto>>
    {
        public EpisodeBySeasonIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetEpisodeBySeasonIdQuery> validator, ILogger<GetEpisodeBySeasonIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<EpisodeDto>>> HandleRequest(GetEpisodeBySeasonIdQuery request, CancellationToken cancellationToken)
        {
            var episodes = await OscarContext.Episodes.Where(x => x.SeasonId == request.SeasonId).ToListAsync();

            return Result.Ok(Mapper.Map<List<EpisodeDto>>(episodes));
        }

    }
}
