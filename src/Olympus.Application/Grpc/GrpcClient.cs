using Grpc.Net.Client;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Grpc.Services;
using ProtoBuf.Grpc.Client;

namespace Olympus.Application.Grpc;

public class GrpcClient(OlympusConfig config) : IGrpcClient, IDisposable
{
  private readonly GrpcChannel _channel = GrpcChannel.ForAddress(config.ApiHost);

  public void Dispose()
  {
    _channel.Dispose();
    GC.SuppressFinalize(this);
  }

  public Task<TalkWithGmResponse> TalkWithGmAsync(
    TalkWithGmRequest request,
    CancellationToken cancellationToken = default)
  {
    return _channel.CreateGrpcService<IAiApiService>().TalkWithGmAsync(request, cancellationToken);
  }
}
