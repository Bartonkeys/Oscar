using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Oscar.Mrit.Features.Integrator.Commands;

namespace Oscar.MRIT.Function
{
    public class MritFunction
    {
        private static bool isRunning = false;
        private readonly IMediator _mediator;
        private readonly ILogger<MritFunction> _logger;

        public MritFunction(IMediator mediator, ILogger<MritFunction> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName(nameof(MritFunction))]
        [FixedDelayRetry(5, "00:00:10")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
        {
            try
            {
                if (isRunning) return;
                isRunning = true;

                
                var result = await _mediator.Send(new FelixOnMusicIntegrationCommand());
                if (result.IsSuccess)
                    _logger.LogInformation("Success");
                else
                    _logger.LogError(result.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                isRunning = false;
            }
        }
    }
}
