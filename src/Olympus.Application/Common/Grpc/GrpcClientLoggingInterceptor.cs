using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common.Grpc;

public sealed partial class GrpcClientLoggingInterceptor(ILoggerFactory loggerFactory) : Interceptor
{
  private readonly ILogger _logger = loggerFactory.CreateLogger<GrpcClientLoggingInterceptor>();

  public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
      TRequest request,
      ClientInterceptorContext<TRequest, TResponse> context,
      AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
  {
    LogStartingCall(_logger, context.Host ?? string.Empty, context.Method.Type.ToString(), context.Method.Name);
    try
    {
      var response = continuation(request, context);
      LogCallSucceeded(_logger, context.Host ?? string.Empty, context.Method.Type.ToString(), context.Method.Name, response);
      return response;
    }
    catch (Exception ex)
    {
      LogCallFailed(_logger, context.Host ?? string.Empty, context.Method.Type.ToString(), context.Method.Name, ex);
      throw;
    }
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Starting call. Host: {Host} Type/Method: {Type} / {Method}")]
  public static partial void LogStartingCall(ILogger logger, string host, string type, string method);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Call succeeded. Host: {Host} Type/Method: {Type} / {Method}. {@Response}")]
  public static partial void LogCallSucceeded(ILogger logger, string host, string type, string method, object response);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Call failed. Host: {Host} Type/Method: {Type} / {Method}")]
  public static partial void LogCallFailed(ILogger logger, string host, string type, string method, Exception incException);
}
