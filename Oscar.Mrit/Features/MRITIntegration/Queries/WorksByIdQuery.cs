using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;
using Oscar.Mrit.Features.Common;
using Oscar.Mrit.Features.MRITIntegration.Common;
using static Oscar.Mrit.Features.MRITIntegration.Queries.FelixWorksQueryHandler;

namespace Oscar.Mrit.Features.MRITIntegration.Queries
{
    public class WorksByIdQuery : IRequest<Result<IEnumerable<ProductionModel>>>
    {
        public IEnumerable<int> WorksIds = new List<int>();
    }

    public class WorksByIdQueryHandler : AbstractBaseHandler<WorksByIdQuery, IEnumerable<ProductionModel>>
    {
        private readonly MritMapperFactory _mritMapperFactory;
        private readonly IEqualityComparer<VwOnMusicFelixWorks> _worksComparer = new WorksComparer();

        public WorksByIdQueryHandler(OscarContext dbContext, IMapper mapper, FluentValidation.IValidator<WorksByIdQuery> validator, MritMapperFactory mritMapperFactory, ILogger<WorksByIdQuery> logger) 
            : base(dbContext, mapper, validator, logger)
        {
            _mritMapperFactory = mritMapperFactory;
        }

        protected override async Task<Result<IEnumerable<ProductionModel>>> HandleRequest(WorksByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail<IEnumerable<ProductionModel>>(validationResult.ToString());

            var felixWorks = (await OscarContext.VwOnMusicFelixWorks
                    .AsNoTracking()
                    .Where(x => request.WorksIds.Contains(x.WorksId) &&
                         (x.SerialLevel == (int)SerialLevel.SeriesHeader || x.SerialLevel == (int)SerialLevel.OneOff))
                    .ToListAsync())
                    .Distinct(_worksComparer);

            var mritProductionModels = new List<ProductionModel>();
            foreach (var felixWork in felixWorks)
            {
                var mritMapper = _mritMapperFactory.Create(felixWork);
                var mritProductionModel = await mritMapper.MapFrom();
                mritProductionModels.Add(mritProductionModel);
            }

            return Result.Ok(mritProductionModels.AsEnumerable());
        }
    }
}
