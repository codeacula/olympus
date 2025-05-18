using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common.Behaviors;

public partial class ErrorHandlingBehavior<TRequest, TResponse>(ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  private readonly ILogger<ErrorHandlingBehavior<TRequest, TResponse>> _logger = logger;

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    try
    {
      return await next(cancellationToken);
    }
    catch (OlympusValidationException ex)
    {
      LogValidationError(_logger, typeof(TRequest).Name, ex.Errors.Select(e => e.Value.Aggregate((a, b) => $"{a}, {b}")));
      throw; // Re-throw for gRPC layer to handle
    }
    catch (OlympusNotFoundException ex)
    {
      LogResourceNotFound(_logger, typeof(TRequest).Name, ex);
      throw; // Re-throw
    }
    // Add catches for other specific Olympus exceptions if you want special logging
    catch (Exception ex)
    {
      LogUnhandledException(_logger, typeof(TRequest).Name, ex);
      throw; // Re-throw for gRPC layer to handle as a generic error
    }
  }

  [LoggerMessage(
      Level = LogLevel.Warning,
      EventId = 0,
      Message = "Validation failed for request {RequestName}: {Errors}")]
  private static partial void LogValidationError(ILogger logger, string requestName, IEnumerable<string> errors);

  [LoggerMessage(
      Level = LogLevel.Error,
      EventId = 0,
      Message = "Unhandled exception occurred for request {RequestName}")]
  private static partial void LogUnhandledException(ILogger logger, string requestName, Exception ex);

  [LoggerMessage(
      Level = LogLevel.Warning,
      EventId = 1,
      Message = "Resource not found for request {RequestName}")]
  private static partial void LogResourceNotFound(ILogger logger, string requestName, Exception ex);
}
