using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Common.Grpc;

namespace Olympus.Bot.Discord.Modules;

public partial class TestInteractionModule(
    IGrpcClient grpcClient,
    ILogger<TestInteractionModule> logger
  ) : BaseInteractionModule<TestInteractionModule>(grpcClient, logger)
{
  [SlashCommand("testinteraction", "Test Olympus")]
  public async Task<string> TestInteractionAsync(string interactionText)
  {
    var request = new TalkWithGmRequest(interactionText);
    var response = await GrpcClient.AiApiService.TalkWithGmAsync(request);

    return response is null ? HandleFailure("Unknown error occurred") : response.Response;
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 0,
      Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string interactionText);
}
