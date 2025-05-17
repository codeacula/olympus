using Olympus.Application.Ai.Interactions.TalkWithGm;

namespace Olympus.Application.Grpc;

public interface IGrpcClient
{
  Task<TalkWithGmResponse> TalkWithGmAsync(
    TalkWithGmRequest request,
    CancellationToken cancellationToken = default);
}
