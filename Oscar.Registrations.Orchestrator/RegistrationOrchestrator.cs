using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Registration.Commands;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Registrations.Orchestrator
{
    public class RegistrationOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegistrationOrchestrator> _logger;

        public RegistrationOrchestrator(IMediator mediator, ILogger<RegistrationOrchestrator> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("RegistrationOrchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var message = context.GetInput<string>();
            var batchId = new Guid(message);
            var allClientsTasks = new List<Task<string>>();

            var allClients = await context.CallActivityAsync<IEnumerable<int>>("GetClients", batchId);

            allClientsTasks = await ProcessClients(context, batchId, allClients);

            //this may be redundant but still added this to ensure all clients are processed before moving to next line to read file results
            await Task.WhenAll(allClientsTasks); 

            var fileResults = allClientsTasks.Select(r => r.Result).Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (fileResults.Any())
            {
                await context.CallActivityAsync("StitchOrZipFiles", new BatchFileResults
                {
                    BatchId = batchId,
                    FileResults = fileResults
                });
            }
        }

        private static async Task<List<Task<string>>> ProcessClients(IDurableOrchestrationContext context, Guid batchId, IEnumerable<int> allClients)
        {
            var allTasks = new List<Task<string>>();
            foreach (var chunk in allClients.Chunk(Constants.Default.ThreadSize))
            {
                //to execute all the tasks from each chunk parallely
                var parallelTasks = new List<Task<string>>();
                foreach (var client in chunk)
                {
                    parallelTasks.Add(context.CallActivityAsync<string>("ProcessClient",
                        new BatchClient
                        {
                            BatchId = batchId,
                            ClientId = client
                        }));
                }

                //wait for all tasks in each chunk to finish before starting next chunk
                await Task.WhenAll(parallelTasks); 

                // Add completed tasks from the current chunk to the allTasks list
                allTasks.AddRange(parallelTasks);
            }

            return allTasks;
        }

        [FunctionName("GetClients")]
        public async Task<IEnumerable<int>> GetClients([ActivityTrigger] Guid batchId, ILogger log)
        {
            var result = await _mediator.Send(new GetClientsForBatchQuery { BatchId = batchId });

            return result.IsSuccess ? result.Value.Select(c => c.Id) : new List<int>();
        }

        [FunctionName("ProcessClient")]
        public async Task<string> ProcessClient([ActivityTrigger] BatchClient batchClient, ILogger log)
        {
            try
            {
                var result = await _mediator.Send(new RegistrationCommand
                    { BatchId = batchClient.BatchId, ClientId = batchClient.ClientId });

                return result.IsSuccess ? result.Value : string.Empty;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return string.Empty;
            }
        }

        [FunctionName("StitchOrZipFiles")]
        public async Task StitchOrZipFiles([ActivityTrigger]BatchFileResults batchFileResults, ILogger log)
        {
            await _mediator.Send(new StitchOrZipRegistrationsCommand { FileResults = batchFileResults.FileResults, BatchId = batchFileResults.BatchId });
        }

        public class BatchClient
        {
            public Guid BatchId { get; set; }
            public int ClientId { get; set; }
        }

        public class BatchFileResults
        {
            public List<string> FileResults { get; set; }
            public Guid BatchId { get; set; }
        }
    }
}