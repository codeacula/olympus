using Olympus.Application.Ai.Interactions.TalkWithGm;

namespace Olympus.Application.Ai.Services;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default)
  {
    var result = await mediator.Send(request, cancellationToken);

    return result ?? throw new OlympusInvalidResponseException();
  }
}
