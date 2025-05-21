using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Olympus.Application.Ai.Services;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Common.Grpc;

public class GrpcClient : IGrpcClient, IDisposable
{
  public IAiGrpcService AiApiService { get; }
  private readonly GrpcChannel _channel;

  public GrpcClient(GrpcChannel channel, GrpcClientLoggingInterceptor GrpcClientLoggingInterceptor)
  {
    _channel = channel;
    var invoker = _channel.Intercept(GrpcClientLoggingInterceptor);
    AiApiService = invoker.CreateGrpcService<IAiGrpcService>();
  }

  public void Dispose()
  {
    _channel.Dispose();
    GC.SuppressFinalize(this);
  }
}
