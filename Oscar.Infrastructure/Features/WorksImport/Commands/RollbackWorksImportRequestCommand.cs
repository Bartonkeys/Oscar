using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class RollbackWorksImportRequestCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class RollbackWorksImportRequestCommandHandler : SimpleAbstractBaseHandler<RollbackWorksImportRequestCommand>
    {
        private readonly IQueueService _queueService;

        public RollbackWorksImportRequestCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<RollbackWorksImportRequestCommand> validator,
            ILogger<RollbackWorksImportRequestCommand> logger,
            IQueueService queueService
        ) : base(oscarContext, mapper, validator, logger)
        {
            _queueService = queueService;
        }

        protected override async Task<Result> HandleRequest(RollbackWorksImportRequestCommand request,
            CancellationToken cancellationToken)
        {
            var workImportRequest =
                (await OscarContext.WorksImportRequests.SingleOrDefaultAsync(r => r.Id == request.Id,
                    cancellationToken)).ToMaybe();

            if (!workImportRequest.HasValue)
                return Result.Fail("Request Not Found");

            workImportRequest.Value.Status = WorksImportRequestStatus.Rollback;

            await OscarContext.SaveChangesAsync(cancellationToken);

            var worksImportQueueDto = new WorksImportQueueDto
            {
                Id = request.Id,
                Status = WorksImportRequestStatus.Rollback
            };

            var queueResult = await _queueService.SendAsync(QueueName.WORKSIMPORT, JsonConvert.SerializeObject(worksImportQueueDto), cancellationToken);
            return queueResult.IsFailure ? Result.Fail(queueResult.Error) : Result.Ok();
        }
    }
}