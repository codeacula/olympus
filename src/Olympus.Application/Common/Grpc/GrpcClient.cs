using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Olympus.Application.Ai.Services;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Common.Grpc;

public class GrpcClient : IGrpcClient, IDisposable
{
  public IAiGrpcService AiApiService { get; }
  private readonly GrpcChannel _channel;
  private readonly ILogger<GrpcClient> _logger;

  public GrpcClient(IOptions<GrpcHostConfig> config, GrpcLoggingInterceptor grpcLoggingInterceptor)
  {
    _channel = GrpcChannel.ForAddress(config.Value.ApiHost);
    var invoker = _channel.Intercept(grpcLoggingInterceptor);
    AiApiService = invoker.CreateGrpcService<IAiGrpcService>();
  }

  public void Dispose()
  {
    _channel.Dispose();
    GC.SuppressFinalize(this);
  }
}
