using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public interface IAiApiService
{
  Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default);
}
