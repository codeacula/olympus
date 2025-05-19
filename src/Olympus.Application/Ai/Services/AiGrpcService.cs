using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Ai.Services;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default)
  {
    var result = await mediator.Send(request, callContext.CancellationToken);

    return result ?? throw new OlympusInvalidResponseException();
  }
}
