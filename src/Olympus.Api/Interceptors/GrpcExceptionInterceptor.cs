
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

      throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
    }
    catch (OlympusNotFoundException ex)
    {
      throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
    }
    // ... other custom exceptions ...
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled gRPC exception");
      throw new RpcException(new Status(StatusCode.Internal, "Internal server error."));
    }
  }

  [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Validation Exception: {Message}")]
  public static partial void LogValidationException(string message, Exception ex);
}
