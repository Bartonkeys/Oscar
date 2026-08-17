using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.WorksImport.Services;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class ResubmitWorksImportRequestCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class UpdateWorksImportRequestCommandHandler : SimpleAbstractBaseHandler<ResubmitWorksImportRequestCommand>
    {
        private readonly IQueueService _queueService;

        public UpdateWorksImportRequestCommandHandler(
            IQueueService queueService,
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<ResubmitWorksImportRequestCommand> validator, 
            ILogger<ResubmitWorksImportRequestCommand> logger) : base(oscarContext, mapper, validator, logger) 
        {
            _queueService = queueService;
        }

        protected override async Task<Result> HandleRequest(ResubmitWorksImportRequestCommand request, CancellationToken cancellationToken)
        {
            var workImportRequest =
                (await OscarContext.WorksImportRequests.SingleOrDefaultAsync(r => r.Id == request.Id,
                    cancellationToken)).ToMaybe();

            if (!workImportRequest.HasValue)
                return Result.Fail("Request Not Found");

            workImportRequest.Value.Status = WorksImportRequestStatus.ReSubmit;

            await OscarContext.SaveChangesAsync(cancellationToken);

            var worksImportQueueDto = new WorksImportQueueDto
            {
                Id = request.Id,
                Status = WorksImportRequestStatus.ReSubmit
            };

            var queueResult = await _queueService.SendAsync(QueueName.WORKSIMPORT, JsonConvert.SerializeObject(worksImportQueueDto), cancellationToken);
            return queueResult.IsFailure ? Result.Fail(queueResult.Error) : Result.Ok();

        }
    }
}
