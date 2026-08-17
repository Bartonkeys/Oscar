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

namespace Oscar.Infrastructure.Features.Conflict.Queries
{
    public class GetConflictsByWorksIdQuery : BaseTableQuery, IRequest<Result<List<ConflictDto>>>
    {
        public int WorksId { get; set; }
    }

    public class GetConflictsByWorksIdQueryHandler : AbstractBaseHandler<GetConflictsByWorksIdQuery, List<ConflictDto>>
    {
        public GetConflictsByWorksIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetConflictsByWorksIdQuery> validator, ILogger<GetConflictsByWorksIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<ConflictDto>>> HandleRequest(GetConflictsByWorksIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var conflicts = await OscarContext
                .Conflicts
                .AsNoTracking()
                .Include(s => s.Society)
                .Where(c => c.Works.Id == request.WorksId)
                .ToListAsync();

            if (conflicts == null)
                return Result.Fail<List<ConflictDto>>("Not found");

            Logger.LogInformation((int)EpisodeFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<List<ConflictDto>>(conflicts));
        }
    }
}
