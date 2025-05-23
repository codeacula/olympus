using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(ILogger<IOlympusApplication> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  private readonly ILogger<IOlympusApplication> _logger = logger;

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    try
    {
      OlympusApplicationLogger.LogHandlingRequest(_logger, typeof(TRequest));
      var response = await next(cancellationToken);
      OlympusApplicationLogger.LogHandledRequest(_logger, typeof(TRequest));
      return response;
    }
    catch (Exception ex)
    {
      OlympusApplicationLogger.LogErrorHandlingRequest(_logger, typeof(TRequest), ex);
      throw;
    }
  }
}
