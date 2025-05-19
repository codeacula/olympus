using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public class AiApiService(IMediator mediator) : IAiApiService
{
  public ValueTask<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default)
  {
    return mediator.Send(request, callContext.CancellationToken);
  }
}
