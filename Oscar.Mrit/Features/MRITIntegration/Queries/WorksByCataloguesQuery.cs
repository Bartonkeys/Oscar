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
using Oscar.Mrit.Features.Common;

namespace Oscar.Mrit.Features.MRITIntegration.Queries
{
    public class WorksByCataloguesQuery: IRequest<Result<IEnumerable<CatalogueWorksDto>>>
    {
        public List<CatalogueDto> Catalogues { get; set; }
    }

    public class WorksByCataloguesQueryHandler : AbstractBaseHandler<WorksByCataloguesQuery, IEnumerable<CatalogueWorksDto>>
    {
        public WorksByCataloguesQueryHandler(OscarContext dbContext, IMapper mapper, IValidator<WorksByCataloguesQuery> validator, ILogger<WorksByCataloguesQuery> logger) : base(dbContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<CatalogueWorksDto>>> HandleRequest(WorksByCataloguesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail<IEnumerable<CatalogueWorksDto>>(validationResult.ToString());

            var cataloguesWorks = new List<CatalogueWorksDto>();
            foreach (var catalogue in request.Catalogues)
            {
                var catalogueWorks = new CatalogueWorksDto
                {
                    CatalogueId = catalogue.CatalogueId,
                    CatalogueName = catalogue.CatalogueName,
                    WorksIds = OscarContext.VwOnMusicFelixWorks
                        .AsNoTracking()
                        .Where(w => OscarContext.OnMusicMatches.Select(m => m.WorksId).Contains(w.WorksId) &&
                                    w.CataloguesId == catalogue.CatalogueId)
                        .Select(f => f.WorksId)
                        .AsEnumerable()
                };
                cataloguesWorks.Add(catalogueWorks);
            }

            return Result.Ok(cataloguesWorks.AsEnumerable());
        }
    }
}
