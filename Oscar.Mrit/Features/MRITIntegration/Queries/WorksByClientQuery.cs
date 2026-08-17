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
    public class WorksByClientQuery: IRequest<Result<ClientWorksDto>>
    {
        public int ClientId { get; set; }
    }

    public class WorksByClientQueryHandler : AbstractBaseHandler<WorksByClientQuery, ClientWorksDto>
    {
        public WorksByClientQueryHandler(OscarContext dbContext, IMapper mapper, IValidator<WorksByClientQuery> validator, ILogger<WorksByClientQuery> logger) : base(dbContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ClientWorksDto>> HandleRequest(WorksByClientQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Fail<ClientWorksDto>(validationResult.ToString());

            var felicClientWorks = OscarContext.VwOnMusicFelixWorks
                .AsNoTracking()
                .Where(w => OscarContext.OnMusicMatches.Select(m => m.WorksId).Contains(w.WorksId) && w.ClientsId == request.ClientId);

            var clientWorksDto = new ClientWorksDto
            {
                ClientId = request.ClientId,
                ClientName = OscarContext.Clients.Single(c => c.Id == request.ClientId).ClientName,
                WorksIds = felicClientWorks.Select(c => c.WorksId)
            };

            return Result.Ok(clientWorksDto);
        }
    }
}
