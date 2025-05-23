using NetCord.Services.ApplicationCommands;
using Olympus.Application.Grpc;
using Olympus.Application.Grpc.Ai.TalkWithGm;

namespace Olympus.Bot.Discord.Modules;

public partial class GmInteractionModule(
    IGrpcClient grpcClient,
    ILogger<GmInteractionModule> logger
  ) : BaseInteractionModule<GmInteractionModule>(grpcClient, logger)
{
  [SlashCommand("greet", "Greet the GM")]
  public async Task<string> GreetAsync(string interactionText)
  {
    try
    {
      return await ExecuteAsync<TalkWithGmRequest, TalkWithGmResponse>(async () =>
      {
        var request = new TalkWithGmRequest(interactionText);
        var response = await GrpcClient.AiApiService.TalkWithGmAsync(request);
        return response;
      });
    }
    catch (Exception ex)
    {
      return HandleFailure("An error occurred");
    }
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Greet command executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string interactionText);
}
