using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Infrastructure.Features.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Equivalence.Function
{
    public class QueueTriggeredFunction
    {
        private readonly ILogger<QueueTriggeredFunction> _logger;

        public QueueTriggeredFunction(ILogger<QueueTriggeredFunction> logger)
        {
            _logger = logger;
        }

        [FunctionName("EquivalenceQueueTrigger")]
        public async Task Run(
            [QueueTrigger(QueueName.EQUIVALENCE, Connection = "oscarstorage")] string message,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            _logger.LogInformation($"Checking status of orchestration for queue item {message}");

            var instanceId = message; // Using the message as the instance ID

            // Check the status of the existing orchestration instance
            var status = await starter.GetStatusAsync(instanceId);

            if (status == null || status.RuntimeStatus == OrchestrationRuntimeStatus.Completed ||
                status.RuntimeStatus == OrchestrationRuntimeStatus.Failed ||
                status.RuntimeStatus == OrchestrationRuntimeStatus.Terminated)
            {
                _logger.LogInformation($"Starting new orchestration for queue item {message}");
                await starter.StartNewAsync("EquivalenceOrchestrator", instanceId, message);
            }
            else
            {
                _logger.LogWarning($"Orchestration with ID {instanceId} is already running with status {status.RuntimeStatus}");
            }
        }
    }
}
