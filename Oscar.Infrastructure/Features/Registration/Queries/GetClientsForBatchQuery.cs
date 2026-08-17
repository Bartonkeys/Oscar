using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Registration.Queries
{
    public class GetClientsForBatchQuery: IRequest<Result<IEnumerable<ClientDto>>>
    {
        public Guid BatchId { get; set; }
    }

    public class GetClientsForBatchQueryHandler : AbstractBaseHandler<GetClientsForBatchQuery, IEnumerable<ClientDto>>
    {
        private readonly IMediator _mediator;

        public GetClientsForBatchQueryHandler(OscarContext oscarContext, IMediator mediator, IMapper mapper, IValidator<GetClientsForBatchQuery> validator, ILogger<GetClientsForBatchQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<IEnumerable<ClientDto>>> HandleRequest(GetClientsForBatchQuery request, CancellationToken cancellationToken)
        {
            var registrationBatch = await OscarContext.RegistrationBatches.FirstOrDefaultAsync(m => m.BatchId == request.BatchId, cancellationToken: cancellationToken);
            if (registrationBatch == null)
            {
                Logger.LogInformation((int)RegistrationFeatureEvent.BatchNotFound, $"Not found {request.BatchId}");
                return Result.Fail<IEnumerable<ClientDto>>(CommandResult.NOTFOUND);
            }

            if (registrationBatch.IsAllClients)
            {
                var allClients =
                    OscarContext
                        .Clients
                        .AsNoTracking()
                        .Where(c => c.Societies.Any(s => s.Id == registrationBatch.SocietyId) 
                                    && (c.Status == Status.Active_Consolidated || c.Status == Status.Active_In_Term || c.Status == Status.Active_Lapsed));

                return Result.Ok(allClients.Select(c => Mapper.Map<ClientDto>(c)).AsEnumerable());
            }

            var client = await _mediator.Send(new GetClientByIdQuery { Id = registrationBatch.ClientId.Value }, cancellationToken);
            if (client.IsFailure) return Result.Fail < IEnumerable<ClientDto> > (client.Error);
            var clientList = new List<ClientDto> { client.Value };
            return Result.Ok(clientList.AsEnumerable());
        }
    }
}
