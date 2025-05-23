using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Ai.Interfaces;

namespace Olympus.Application.Ai.Services;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(request);

    var command = new TalkWithGmCommand(request.Value);

    var result = await mediator.Send(command, cancellationToken) ?? throw new OlympusInvalidResponseException();
    return new TalkWithGmResponse() { Value = result.Value };
  }
}
