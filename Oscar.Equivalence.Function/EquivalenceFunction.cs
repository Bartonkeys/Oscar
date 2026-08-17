using System;
using System.Threading.Tasks;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Equivalence.Commands;

namespace Oscar.Equivalence.Function
{
    public class EquivalenceFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EquivalenceFunction> _logger;

        public EquivalenceFunction(IMediator mediator, ILogger<EquivalenceFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("ProcessEquivalenceActivity")]
        public async Task<string> ProcessEquivalenceActivity([ActivityTrigger] string message, ILogger log)
        {
            if (string.IsNullOrEmpty(message)) return "Fail";

            var processEquivalenceCommand = new ProcessEquivalenceCommand
            {
                RequestId = new Guid(message)
            };
            var result = await _mediator.Send(processEquivalenceCommand);

            if (result.IsSuccess)
            {
                _logger.LogInformation((int)FunctionEvent.EquivalenceProcessor, $"Equivalence Processing success success for queue item {message}");
            }
            else
            {
                _logger.LogWarning((int)FunctionEvent.EquivalenceError, $"Equivalence Processing failed for queue item {message}");
                throw new Exception($"Equivalence Processing failed for queue item {message} : Exception {result.Error}");
            }

            return "Success";
        }
    }
}

