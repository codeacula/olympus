using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common;

public static partial class OlympusApplicationLogger
{
  [LoggerMessage(Level = LogLevel.Information, Message = "Handling request {requestType}")]
  public static partial void LogHandlingRequest(ILogger<IOlympusApplication> incLogger, Type requestType);

  [LoggerMessage(Level = LogLevel.Information, Message = "Handled request {requestType}")]
  public static partial void LogHandledRequest(ILogger<IOlympusApplication> incLogger, Type requestType);

  [LoggerMessage(Level = LogLevel.Error, Message = "Error handling request {requestType}")]
  public static partial void LogErrorHandlingRequest(ILogger<IOlympusApplication> incLogger, Type requestType, Exception ex);

  [LoggerMessage(Level = LogLevel.Error, Message = "Validation failed for request {requestType}: {Errors}")]
  public static partial void LogValidationError(ILogger logger, Type requestType, IEnumerable<string> errors);

  [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception occurred for request {requestType}")]
  public static partial void LogUnhandledException(ILogger logger, Type requestType, Exception ex);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Resource not found for request {requestType}")]
  public static partial void LogResourceNotFound(ILogger logger, Type requestType, Exception ex);

  [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while processing the AI request {requestType}")]
  public static partial void LogAiException(ILogger logger, Type requestType, Exception ex);

  [LoggerMessage(Level = LogLevel.Error, Message = "Invalid response from {requestType}")]
  public static partial void LogInvalidResponse(ILogger logger, Type requestType, Exception ex);

  [LoggerMessage(Level = LogLevel.Error, Message = "An authentication error occurred while processing the request {requestType}")]
  public static partial void LogAuthenticationError(ILogger logger, Type requestType, Exception ex);
}
