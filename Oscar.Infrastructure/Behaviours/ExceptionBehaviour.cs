using MediatR;
using Microsoft.Extensions.Logging;

namespace Oscar.Infrastructure.Behaviours
{
    public class ExceptionBehaviour
    {
        public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

            public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
            {
                _logger = logger;
            }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
            {
                try
                {
                    var response = await next();
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception at {typeof(TResponse).Name} of {typeof(TRequest).Name} at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff}");
                    throw;
                }
            }
        }
    }
}
