using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Registration.Commands;


namespace Oscar.Registration.WebJob
{
    public class RegistrationFunction
    {
        private IMediator _mediator;
        private ILogger<RegistrationFunction> _logger;

        public RegistrationFunction(IMediator mediator, ILogger<RegistrationFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Singleton]
        public async Task ProcessQueueMessage([QueueTrigger(QueueName.TEST)] string message, ILogger logger)
        {
            var registrationCommand = new RegistrationCommand
            {
                BatchId = new Guid(message)
            };

            try
            {
                var result = await _mediator.Send(registrationCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation((int)FunctionEvent.Registration, $"Registration success for queue item {message}");
                }
                else
                {
                    _logger.LogWarning((int)FunctionEvent.RegistrationError, $"Registration failed for queue item {message}");
                    throw new Exception($"Registration failed for queue item {message} : Exception {result.Error}");
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
