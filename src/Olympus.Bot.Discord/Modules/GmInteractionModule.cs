using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Grpc;

namespace Olympus.Bot.Discord.Modules;

public class GmInteractionModule(IGrpcClient grpcClient) : ApplicationCommandModule<ApplicationCommandContext>
{
  private readonly IGrpcClient _grpcClient = grpcClient;

  [SlashCommand("talk", "Talk with the GM")]
  public async Task<string> TalkWithGmAsync(string interactionText)
  {
    var grcpRequest = new TalkWithGmRequest { Value = interactionText };
    var response = await _grpcClient.AiApiService.TalkWithGmAsync(grcpRequest);
    return response.Value;
  }
}
