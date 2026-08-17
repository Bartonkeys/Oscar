using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Registrations.Orchestrator
{
    public class RegistrationFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegistrationFunction> _logger;

        public RegistrationFunction(IMediator mediator, ILogger<RegistrationFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("RegistrationFunction")]
        public async Task Run([QueueTrigger(QueueName.REGISTRATION, Connection = "oscarstorage")] string message,
            [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            var instanceId = await starter.StartNewAsync<string>("RegistrationOrchestrator", message);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }
    }
}
