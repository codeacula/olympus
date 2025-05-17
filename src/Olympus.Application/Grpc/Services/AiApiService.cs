using Olympus.Application.Ai.Errors;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Ai.Services.AiInteractionService;
using Olympus.Application.Common.Types;
using ProtoBuf.Grpc;

namespace Olympus.Application.Grpc.Services;

public class AiApiService(IAiInteractionService aiInteractionService) : IAiApiService
{
  public Task<OlympusResult<TalkWithGmResponse, FailedToGetResponseError>> TalkWithGmAsync(
    TalkWithGmRequest request,
    CallContext callContext = default)
  {
    return aiInteractionService.SendAiRequestAsync<TalkWithGmResponse, FailedToGetResponseError>(request, callContext.CancellationToken);
  }
}
