using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Ai.Services.AiInteractionService;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public class AiApiService(IAiInteractionService aiInteractionService) : IAiApiService
{
  public Task<TalkWithGmResponse> TalkWithGmAsync(
    TalkWithGmRequest request,
    CallContext callContext = default)
  {
    return aiInteractionService.SendAiRequestAsync(request, callContext.CancellationToken);
  }
}
