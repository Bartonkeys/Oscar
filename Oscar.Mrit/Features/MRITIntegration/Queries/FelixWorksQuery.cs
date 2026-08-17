using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.Constants;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;
using Oscar.Mrit.Features.Common;
using Oscar.Mrit.Features.MRITIntegration.Common;

namespace Oscar.Mrit.Features.MRITIntegration.Queries
{
    public class FelixWorksQuery: IRequest<Result<IEnumerable<ProductionModel>>>
    {
        public int Take { get; set; } = 20;
        public MatchStatus MatchStatus { get; set; } = MatchStatus.Success;
    }

    public class FelixWorksQueryHandler : AbstractBaseHandler<FelixWorksQuery, IEnumerable<ProductionModel>>
    {
        private readonly MritMapperFactory _mritMapperFactory;
        private readonly BlackListDto _blackList;
        private readonly IEqualityComparer<VwOnMusicFelixWorks> _worksComparer = new WorksComparer();

        public FelixWorksQueryHandler(OscarContext dbContext, IMapper mapper, IValidator<FelixWorksQuery> validator, MritMapperFactory mritMapperFactory, IOptions<BlackListDto>  blackList, ILogger<FelixWorksQuery> logger) 
            : base(dbContext, mapper, validator, logger)
        {
            _mritMapperFactory = mritMapperFactory;
            _blackList = blackList.Value;
        }

        protected override async Task<Result<IEnumerable<ProductionModel>>> HandleRequest(FelixWorksQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail< IEnumerable < ProductionModel >> (validationResult.ToString());

            if(_blackList.ClientIds == null || _blackList.ClientIds.Count == 0)  
                _blackList.ClientIds = new List<int>()
                {
                    1074,
                    1073,
                    1059,
                    1075,
                    1072
                };

            var felixWorks = OscarContext.VwOnMusicFelixWorks
                .AsNoTracking()
                .Where(GetPredicate(request))
                .Take(request.Take)
                ?.ToList()
                ?.Distinct(_worksComparer);

            var mritProductionModels = new List<ProductionModel>();
            foreach ( var felixWork in felixWorks)
            {
                var mritMapper = _mritMapperFactory.Create(felixWork);
                var mritProductionModel = await mritMapper.MapFrom();
                mritProductionModels.Add(mritProductionModel);
                if (request.MatchStatus != MatchStatus.Success)
                    await IncrementMatchStatusRetry(felixWork.WorksId);
            }

            return Result.Ok(mritProductionModels.AsEnumerable());
        }

        private async Task IncrementMatchStatusRetry(int worksId)
        {
            var matchStatus = await OscarContext.OnMusicMatches.SingleAsync(m => m.WorksId == worksId);
            matchStatus.RetryCount++;
            await OscarContext.SaveChangesAsync();
        }

        private Expression<Func<VwOnMusicFelixWorks, bool>> GetPredicate(FelixWorksQuery request)
        {
            return request.MatchStatus switch
            {
                MatchStatus.Success => w => !OscarContext.OnMusicMatches.Any(m => m.WorksId == w.WorksId)
                                            && !_blackList.ClientIds.Contains(w.WorksId)
                                            && (w.SerialLevel == (int)SerialLevel.SeriesHeader ||
                                                w.SerialLevel == (int)SerialLevel.OneOff),
                MatchStatus.Error => w => OscarContext.OnMusicMatches
                    .Where(m => m.OnMusicMatchStatusId == (int)MatchStatus.Error && m.RetryCount <= FelixConstants.Retry.Count)
                    .Select(m => m.WorksId).Contains(w.WorksId),
                MatchStatus.Duplicate => w => OscarContext.OnMusicMatches
                    .Where(m => m.OnMusicMatchStatusId == (int)MatchStatus.Duplicate && m.RetryCount <= FelixConstants.Retry.Count)
                    .Select(m => m.WorksId).Contains(w.WorksId),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        internal class WorksComparer : IEqualityComparer<VwOnMusicFelixWorks>
        {
            public bool Equals(VwOnMusicFelixWorks x, VwOnMusicFelixWorks y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.WorksId == y.WorksId;
            }

            public int GetHashCode(VwOnMusicFelixWorks obj)
            {
                return obj.WorksId;
            }
        }
    }
}
