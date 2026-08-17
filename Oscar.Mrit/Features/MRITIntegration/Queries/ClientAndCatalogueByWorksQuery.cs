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
    public class ClientAndCatalogueByWorksQuery : IRequest<Result<IEnumerable<ClientCataloguesDto>>>
    {
        public List<int> WorksIds { get; set; }
    }

    public class ClientAndCatalogueByWorksHandler : AbstractBaseHandler<ClientAndCatalogueByWorksQuery, IEnumerable<ClientCataloguesDto>>
    {

        public ClientAndCatalogueByWorksHandler(OscarContext dbContext, IMapper mapper, IValidator<ClientAndCatalogueByWorksQuery> validator, ILogger<ClientAndCatalogueByWorksQuery> logger) : base(dbContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<ClientCataloguesDto>>> HandleRequest(ClientAndCatalogueByWorksQuery request, CancellationToken cancellationToken)
        {
            var clientsAndCatalogues = await OscarContext.VwOnMusicFelixWorks
                .Where(x => request.WorksIds.Contains(x.WorksId))
                .Select(x =>
                new ClientCatalogueQueryObject
                {
                    ClientName = x.ClientName,
                    ClientsId = x.ClientsId,
                    CataloguesId = x.CataloguesId,
                    CatalogueName = x.CatalogueName
                })
                .Distinct()
                .ToListAsync();

            return Result.Ok(CatalogueByClientHelper.GroupClientsIntoCatalogues(clientsAndCatalogues));
        }
    }
}
