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

namespace Oscar.Infrastructure.Features.Matching.Queries
{
    public class GetMatchRequestByIdQuery : BaseTableQuery, IRequest<Result<MatchRequestDto>>
    {
        public int Id { get; set; }
    }

    public class GetMatchRequestByIdQueryHandler : AbstractBaseHandler<GetMatchRequestByIdQuery, MatchRequestDto>
    {
        public GetMatchRequestByIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetMatchRequestByIdQuery> validator, ILogger<GetMatchRequestByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<MatchRequestDto>> HandleRequest(GetMatchRequestByIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var matchRequest = await OscarContext.MatchRequests.FirstOrDefaultAsync(w => w.Id == request.Id);

            Logger.LogInformation((int)MatchRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<MatchRequestDto>(matchRequest));
        }

    }
}
