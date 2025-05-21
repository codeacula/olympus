using Olympus.Application.Grpc.Ai.TalkWithGm;

namespace Olympus.Application.Grpc.Ai;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default)
  {
    var result = await mediator.Send(request, cancellationToken);

    return result ?? throw new OlympusInvalidResponseException();
  }
}
