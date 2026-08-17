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
    public class GetClientByWorkIdQuery: BaseTableQuery, IRequest<Result<ClientDto>>
    {
        public int Id { get; set; }
    }

    public class ClientByWorkIdHandler : AbstractBaseHandler<GetClientByWorkIdQuery, ClientDto>
    {
        public ClientByWorkIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetClientByWorkIdQuery> validator, ILogger<GetClientByWorkIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ClientDto>> HandleRequest(GetClientByWorkIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ClientFeatureEvent.Get, $"GET By WorkId: {request.Id}");
            var client = await OscarContext.Clients.AsNoTracking()
                .Include(i => i.Addresses)
                .Include(i => i.Works)
                .FirstOrDefaultAsync(x => x.Works.Any(w => w.Id == request.Id), cancellationToken: cancellationToken);

            var clientDto = Mapper.Map<ClientDto>(client);
            Logger.LogInformation((int)ClientFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(clientDto);
        }

    }
}
