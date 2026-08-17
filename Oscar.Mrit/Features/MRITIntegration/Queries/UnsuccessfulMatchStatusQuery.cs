using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Data.Context;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.Mrit.Features.Common;

namespace Oscar.Mrit.Features.MRITIntegration.Queries
{
    public class UnsuccessfulMatchStatusQuery: IRequest<Result<List<MatchStatusDto>>>
    {
    }

    public class UnsuccessfulMatchStatusQueryHandler : AbstractBaseHandler<UnsuccessfulMatchStatusQuery, List<MatchStatusDto>>
    {
        public UnsuccessfulMatchStatusQueryHandler(OscarContext dbContext, IMapper mapper, IValidator<UnsuccessfulMatchStatusQuery> validator, ILogger<UnsuccessfulMatchStatusQuery> logger) 
            : base(dbContext, mapper, validator, logger)
        {
        }

        protected async override Task<Result<List<MatchStatusDto>>> HandleRequest(UnsuccessfulMatchStatusQuery request, CancellationToken cancellationToken)
        {
            var results = OscarContext.OnMusicMatches
                .Include(s => s.OnMusicMatchStatus)
                .Where(m => m.OnMusicMatchStatusId == (int) MatchStatus.Error ||
                            m.OnMusicMatchStatusId == (int) MatchStatus.Duplicate)
                .Select(s => new MatchStatusDto
                {
                    WorksId = s.WorksId,
                    MatchStatus = Enum.Parse<MatchStatus>(s.OnMusicMatchStatus.Name),
                    Message = s.Message
                })
                .ToList();

            return Result.Ok(results);
        }
    }
}
