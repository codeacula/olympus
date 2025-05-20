using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Olympus.Application.Common.Grpc;

internal sealed partial class GrpcLoggingInterceptor(ILogger<GrpcLoggingInterceptor> log) : Interceptor
{
  private readonly ILogger<GrpcLoggingInterceptor> _log = log;

  public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
    TRequest request,
    ClientInterceptorContext<TRequest, TResponse> context,
    AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
  {
    LogCallingMethod(_log, context.Method.FullName, context.Host ?? "No Host", request);

    var call = continuation(request, context);

    return new AsyncUnaryCall<TResponse>(
      HandleResponseAsync(call.ResponseAsync, context.Method.FullName),
      call.ResponseHeadersAsync,
      call.GetStatus,
      call.GetTrailers,
      call.Dispose);
  }

  private async Task<T> HandleResponseAsync<T>(Task<T> task, string method)
  {
    try
    {
#pragma warning disable VSTHRD003
      var resp = await task;
#pragma warning restore VSTHRD003
      LogResponse(_log, method, resp ?? new object());
      return resp;
    }
    catch (Exception ex)
    {
      LogError(_log, method, ex);
      throw;
    }
  }

  [LoggerMessage(
    EventId = 1,
    Level = LogLevel.Information,
    Message = "gRPC → Calling {Method} on {Host} with request: {@Request}")]
  private static partial void LogCallingMethod(ILogger logger, string method, string host, object request);

  [LoggerMessage(
    EventId = 2,
    Level = LogLevel.Information,
    Message = "gRPC ← {Method} responded: {@Response}")]
  private static partial void LogResponse(ILogger logger, string method, object response);

  [LoggerMessage(
    EventId = 3,
    Level = LogLevel.Error,
    Message = "gRPC ← {Method} failed: {@Error}")]
  private static partial void LogError(ILogger logger, string method, Exception error);
}
