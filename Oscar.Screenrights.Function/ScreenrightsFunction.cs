using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Screenrights.Commands;

namespace Oscar.Screenrights.Function
{
    public class ScreenrightsFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ScreenrightsFunction> _logger;

        public ScreenrightsFunction(IMediator mediator, ILogger<ScreenrightsFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("ScreenrightsFunction")]
        public async Task Run([QueueTrigger(QueueName.SCREENRIGHTS, Connection = "oscarstorage")] string message, ILogger log)
        {
            var processScreenrightsCommand = new ProcessScreenrightsCommand
            {
                RequestId = new Guid(message)
            };
            var result = await _mediator.Send(processScreenrightsCommand);

            if (result.IsSuccess)
            {
                _logger.LogInformation((int)FunctionEvent.ScreenrightsProcessor, $"Screenrights Processing success success for queue item {message}");
            }
            else
            {
                _logger.LogWarning((int)FunctionEvent.ScreenrightsError, $"Screenrights Processing failed for queue item {message}");
                throw new Exception($"Screenrights Processing failed for queue item {message} : Exception {result.Error}");
            }
        }
    }
}

