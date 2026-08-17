using BartonKeys.Functional;

namespace Oscar.Infrastructure.Features.Common.Contracts
{
    public interface IQueueService
    {
        Task<Result> SendAsync(string queueName, string message, CancellationToken cancellationToken);
    }
}
