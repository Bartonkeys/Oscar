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
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Matching.Queries
{
    public class GetMatchResultByIdQuery : BaseTableQuery, IRequest<Result<MatchResultDto>>
    {
        public int Id { get; set; }
    }

    public class GetMatchResultByIdQueryHandler : AbstractBaseHandler<GetMatchResultByIdQuery, MatchResultDto>
    {
        private readonly IImporter _importer;

        public GetMatchResultByIdQueryHandler(
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<GetMatchResultByIdQuery> validator, 
            ILogger<GetMatchResultByIdQuery> logger,
            IImporter importer) : base(oscarContext, mapper, validator, logger)
        {
            _importer = importer;
        }

        protected override async Task<Result<MatchResultDto>> HandleRequest(GetMatchResultByIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var matchRequest = await OscarContext.MatchRequests.FirstOrDefaultAsync(w => w.Id == request.Id);

            if(matchRequest == null)
            {
                Logger.LogInformation((int)MatchResultFeatureEvent.GetNotFound, CommandResult.NOTFOUND);
                return Result.Ok(Mapper.Map<MatchResultDto>(matchRequest));
            }
            
            var matchResult = Mapper.Map<MatchResultDto>(matchRequest);
            var importFileResult = _importer.ImportMatchBlobAsBytes($"{matchRequest.Reference}_MATCHED.csv");
            if (importFileResult.IsSuccess)
            {
                matchResult.FileBytes = importFileResult.Value;
                Logger.LogInformation((int)MatchResultFeatureEvent.Get, CommandResult.SUCCESS);
            }
            else
            {
                Logger.LogInformation((int)MatchResultFeatureEvent.Get, CommandResult.NOTFOUND);
            }
            return Result.Ok(matchResult);
        }

    }
}
