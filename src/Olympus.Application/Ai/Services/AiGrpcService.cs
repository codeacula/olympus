using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Ai.Services;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default)
  {
    return mediator.Send(request, callContext.CancellationToken);
  }
}
