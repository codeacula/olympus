using MediatR;
using Olympus.Application.Grpc;
using Olympus.Application.Grpc.Ai.TalkWithGm;

namespace Olympus.Bot.Discord.Commands.TalkWithGm;

public sealed class TalkWithGmHandler(IGrpcClient grpcClient) : IRequestHandler<TalkWithGmCommand, TalkWithGmResult>
{
  private readonly IGrpcClient _grpcClient = grpcClient;

  public async Task<TalkWithGmResult> Handle(TalkWithGmCommand request, CancellationToken cancellationToken)
  {
    var grpcRequest = new TalkWithGmRequest(request.InteractionText);
    var response = await _grpcClient.AiApiService.TalkWithGmAsync(grpcRequest);
    return response.Response;
  }
}
