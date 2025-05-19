using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Ai.Services;

public interface IAiGrpcService
{
  Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default);
}
