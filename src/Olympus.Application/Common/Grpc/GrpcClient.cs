using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Olympus.Application.Ai.Services;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Common.Grpc;

public class GrpcClient : IGrpcClient, IDisposable
{
  public IAiGrpcService AiApiService { get; }
  private readonly GrpcChannel _channel;

  public GrpcClient(IOptions<GrpcHostConfig> config)
  {
    _channel = GrpcChannel.ForAddress(config.Value.ApiHost);
    AiApiService = _channel.CreateGrpcService<IAiGrpcService>();
  }

  public void Dispose()
  {
    _channel.Dispose();
    GC.SuppressFinalize(this);
  }
}
