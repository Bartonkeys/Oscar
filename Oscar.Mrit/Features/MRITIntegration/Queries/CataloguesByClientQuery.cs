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
using Oscar.Mrit.Features.MRITIntegration.Queries.Helpers;

namespace Oscar.Mrit.Features.MRITIntegration.Queries
{
    public class CataloguesByClientQuery : IRequest<Result<IEnumerable<ClientCataloguesDto>>>
    {
    }

    public class CataloguesByClientQueryHandler : AbstractBaseHandler<CataloguesByClientQuery, IEnumerable<ClientCataloguesDto>>
    {

        public CataloguesByClientQueryHandler(OscarContext dbContext, IMapper mapper,
            IValidator<CataloguesByClientQuery> validator, ILogger<CataloguesByClientQuery> logger) : base(dbContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<ClientCataloguesDto>>> HandleRequest(CataloguesByClientQuery request,
            CancellationToken cancellationToken)
        {
            var felixClients = await OscarContext.VwOnMusicFelixWorks
                .AsNoTracking()
                .Join(OscarContext.OnMusicMatches, w => w.WorksId, m => m.WorksId, (w, m) =>
                new ClientCatalogueQueryObject
                {
                    CataloguesId = w.CataloguesId,
                    CatalogueName = w.CatalogueName,
                    ClientsId = w.ClientsId,
                    ClientName = w.ClientName
                })
                .Distinct()
                .ToListAsync();

            return Result.Ok(CatalogueByClientHelper.GroupClientsIntoCatalogues(felixClients));
        }
    }
}
