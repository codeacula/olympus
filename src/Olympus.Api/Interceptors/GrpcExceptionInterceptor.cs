
using Grpc.Core;
using Grpc.Core.Interceptors;
using Olympus.Application.Common.Exceptions;

namespace Olympus.Api.Interceptors;

public partial class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
  private readonly ILogger<GrpcExceptionInterceptor> _logger = logger;

  public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
      TRequest request, ServerCallContext context,
      UnaryServerMethod<TRequest, TResponse> continuation)
  {
    try { return await continuation(request, context); }
    catch (OlympusValidationException ex)
    {
      LogValidationException(_logger, ex.Message, ex);
      throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
    }
    catch (OlympusNotFoundException ex)
    {
      LogNotFoundException(_logger, ex.Message, ex);
      throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
    }
    catch (Exception ex)
    {
      LogUnhandledException(_logger, ex);
      throw new RpcException(new Status(StatusCode.Internal, "Internal server error."));
    }
  }

  [LoggerMessage(Level = LogLevel.Warning, Message = "Validation Exception: {Message}")]
  public static partial void LogValidationException(ILogger logger, string message, Exception ex);

  [LoggerMessage(Level = LogLevel.Warning, Message = "Not Found Exception: {Message}")]
  public static partial void LogNotFoundException(ILogger logger, string message, Exception ex);

  [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled gRPC exception")]
  public static partial void LogUnhandledException(ILogger logger, Exception ex);
}
