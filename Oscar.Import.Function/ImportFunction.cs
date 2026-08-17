using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Matching.Commands;
using System;
using System.Threading.Tasks;
using BartonKeys.Functional;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.Function
{
    public class ImportFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ImportFunction> _logger;

        public ImportFunction(IMediator mediator, ILogger<ImportFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("ImportFunction")]
        public async Task Run([QueueTrigger(QueueName.WORKSIMPORT, Connection = "oscarstorage")]string message)
        {
            
            var workImportQueueRequest = JsonConvert.DeserializeObject<WorksImportQueueDto>(message);

            var result = Result.Ok();
            switch (workImportQueueRequest!.Status)
            {
                case WorksImportRequestStatus.Rollback:
                    result = await _mediator.Send(new RollbackWorksImportCommand()
                    {
                        Id = workImportQueueRequest.Id
                    });
                    break;
                default:
                    result = await _mediator.Send(new WorksImportCommand
                    {
                        WorksImportRequestId = workImportQueueRequest.Id,
                        Status = workImportQueueRequest.Status
                    });
                    break;
            }

            if (result.IsSuccess)
            {
                _logger.LogInformation((int)FunctionEvent.Match, $"Import success for queue item {workImportQueueRequest.Id}");
            }
            else
            {
                _logger.LogWarning((int)FunctionEvent.MatchError, $"Works Import failed for queue item {workImportQueueRequest.Id}");
            }
        }
    }
}
