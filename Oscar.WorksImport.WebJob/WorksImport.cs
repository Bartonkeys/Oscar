using BartonKeys.Functional;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.WorksImport.Commands;

namespace Oscar.WorksImport.WebJob
{
    public class WorksImport
    {
        private IMediator _mediator;
        private ILogger<WorksImport> _logger;

        public WorksImport(IMediator mediator, ILogger<WorksImport> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Singleton]
        public async Task ProcessQueueMessage([QueueTrigger(QueueName.WORKSIMPORT)] string message, ILogger logger)
        {
            try
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
                    throw new Exception($"Works Import failed for queue item {workImportQueueRequest.Id} : Exception {result.Error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
