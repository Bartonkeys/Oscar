using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Matching.Commands;
using System;
using System.Threading.Tasks;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Function
{
    public class MatchFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MatchFunction> _logger;

        public MatchFunction(IMediator mediator, ILogger<MatchFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("MatchFunction")]
        public async Task Run([QueueTrigger(QueueName.MATCH, Connection = "oscarstorage")]string myQueueItem)
        {
            var matchCommand = new MatchCommand
            {
                Reference = myQueueItem
            };
            var matchResult = await _mediator.Send(matchCommand);

            if (matchResult.IsSuccess)
            {
                _logger.LogInformation((int)FunctionEvent.Match, $"Matching success for queue item {myQueueItem}");
            }
            else
            {
                _logger.LogWarning((int)FunctionEvent.MatchError, $"Matching failed for queue item {myQueueItem}");
                throw new Exception($"Matching failed for queue item {myQueueItem} : Exception {matchResult.Error}");
            }
        }
    }
}
