using Olympus.Application.Ai.Interactions.TalkWithGm;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public class AiApiService(IMediator mediator) : IAiApiService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CallContext callContext = default)
  {
    var result = await mediator.Send(request, callContext.CancellationToken);

    return result ?? throw new OlympusInvalidResponseException();
  }
}
