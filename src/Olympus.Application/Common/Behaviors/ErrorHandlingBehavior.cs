using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common.Behaviors;

public class ErrorHandlingBehavior<TRequest, TResponse>(ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  private readonly ILogger<ErrorHandlingBehavior<TRequest, TResponse>> _logger = logger;

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    try
    {
      return await next(cancellationToken);
    }
    catch (OlympusAiException ex)
    {
      OlympusApplicationLogger.LogAiException(_logger, typeof(TRequest), ex);
      throw;
    }
    catch (OlympusInvalidResponseException ex)
    {
      OlympusApplicationLogger.LogInvalidResponse(_logger, typeof(TRequest), ex);
      throw;
    }
    catch (OlympusUnauthorizedAccessException ex)
    {
      OlympusApplicationLogger.LogAuthenticationError(_logger, typeof(TRequest), ex);
      throw;
    }
    catch (OlympusValidationException ex)
    {
      OlympusApplicationLogger.LogValidationError(_logger, typeof(TRequest), ex.Errors.Select(e => e.Value.Aggregate((a, b) => $"{a}, {b}")));
      throw;
    }
    catch (OlympusNotFoundException ex)
    {
      OlympusApplicationLogger.LogResourceNotFound(_logger, typeof(TRequest), ex);
      throw;
    }
    catch (Exception ex)
    {
      OlympusApplicationLogger.LogUnhandledException(_logger, typeof(TRequest), ex);
      throw;
    }
  }
}
