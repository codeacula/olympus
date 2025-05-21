using Olympus.Application.Grpc.Ai.TalkWithGm;
using ProtoBuf.Grpc.Configuration;

namespace Olympus.Application.Grpc.Ai;

[Service]
public interface IAiGrpcService
{
  Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default);
}
