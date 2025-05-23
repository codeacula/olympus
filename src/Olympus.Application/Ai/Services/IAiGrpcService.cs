using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc.Configuration;

namespace Olympus.Application.Ai.Services;

[Service]
public interface IAiGrpcService
{
  Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default);
}
