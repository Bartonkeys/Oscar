using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using System;
using System.Threading.Tasks;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Function
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
        public async Task Run([QueueTrigger(QueueName.REGISTRATION, Connection = "oscarstorage")]string message)
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
