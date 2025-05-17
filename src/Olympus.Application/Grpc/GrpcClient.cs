using Grpc.Net.Client;
using Olympus.Application.Grpc.Services;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Grpc;

public class GrpcClient : IGrpcClient, IDisposable
{
  public IAiApiService AiApiService { get; }
  private readonly GrpcChannel _channel;

  public GrpcClient(OlympusConfig config)
  {
    _channel = GrpcChannel.ForAddress(config.ApiHost);
    AiApiService = _channel.CreateGrpcService<IAiApiService>();
  }

  public void Dispose()
  {
    _channel.Dispose();
    GC.SuppressFinalize(this);
  }
}
