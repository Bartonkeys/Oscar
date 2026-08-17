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
    public class GetClientByIdQuery: BaseTableQuery, IRequest<Result<ClientDto>>
    {
        public int Id { get; set; }
    }

    public class ClientByIdHandler : AbstractBaseHandler<GetClientByIdQuery, ClientDto>
    {
        public ClientByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetClientByIdQuery> validator, ILogger<GetClientByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ClientDto>> HandleRequest(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ClientFeatureEvent.Get, $"GET By ClientId:{request.Id}");
            var client = await OscarContext.Clients.AsNoTracking()
                .Include(i => i.Addresses)
                .Include(i => i.Societies)
                .Include(c => c.Contract)
                .Include(c => c.Catalogues)
                .Include(c => c.Documents)
                .Include(a => a.ClientAltNames)
                .Include(c => c.Contacts)
                .Include(c => c.CustomerServiceManagers)!
                .ThenInclude(o => o.Operator)
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken: cancellationToken);
            var clientDto = Mapper.Map<ClientDto>(client);
            Logger.LogInformation((int)ClientFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(clientDto);
        }
    }
}
