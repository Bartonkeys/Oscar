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

namespace Oscar.Infrastructure.Features.Clients.Queries
{
    public class GetClientBasicQuery : BaseTableQuery, IRequest<Result<List<ClientBasicDto>>>
    {

    }

    public class ClientBasicHandler : AbstractBaseHandler<GetClientBasicQuery, List<ClientBasicDto>>
    {
        public ClientBasicHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetClientBasicQuery> validator, ILogger<GetClientBasicQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<ClientBasicDto>>> HandleRequest(GetClientBasicQuery request, CancellationToken cancellationToken)
        {
            var clients = OscarContext
                .Clients
                .Include(c => c.Catalogues)
                .AsNoTracking()
                .Select(c => new ClientBasicDto
                {
                    Id = c.Id,
                    ClientName = c.ClientName,
                    Catalogues = c.Catalogues.Select(cat => new CatalogueDto { Id = cat.Id, Name = cat.Name }).ToList()
                });

            Logger.LogInformation((int)StandAloneFeatureEvent.Get, CommandResult.SUCCESS);
            var clientBasicDto = Mapper.Map<List<ClientBasicDto>>(clients);


            return Result.Ok(clientBasicDto);
        }
    }
}
