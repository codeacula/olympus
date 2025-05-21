using Olympus.Application.Grpc.Ai.TalkWithGm;

namespace Olympus.Application.Grpc.Ai;

public class AiGrpcService(IMediator mediator) : IAiGrpcService
{
  public async Task<TalkWithGmResponse> TalkWithGmAsync(TalkWithGmRequest request, CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(request);

    var command = new TalkWithGmCommand(request.InteractionText);
    var result = await mediator.Send(command, cancellationToken);

    var response = new TalkWithGmResponse(result.Response);

    return response ?? throw new OlympusInvalidResponseException();
  }
}
