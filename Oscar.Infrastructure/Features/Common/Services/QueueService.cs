using Azure.Storage.Queues;
using BartonKeys.Functional;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Common.Services
{
    public class QueueService : IQueueService
    {
        private QueueServiceClient _queueServiceClient;
        private ILogger<QueueService> _logger;

        public QueueService(QueueServiceClient queueServiceClient, ILogger<QueueService> logger)
        {
            _queueServiceClient = queueServiceClient;
            _logger = logger;
        }

        public async Task<Result> SendAsync(string queueName, string message, CancellationToken cancellationToken)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);

            var sendResult = await queueClient.SendMessageAsync(message.EncodeBase64(), cancellationToken);
            var queueResponse = sendResult.GetRawResponse();
            if (queueResponse.IsError)
            {
                _logger.LogError((int)AzureStorage.QueueSend, queueResponse.ReasonPhrase);
                return Result.Fail(queueResponse.ReasonPhrase);
            }
            return Result.Ok();

        }
    }
}
