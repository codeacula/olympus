using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Olympus.Application.Common.Grpc;
using Olympus.Application.Grpc.Ai;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Grpc;

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
