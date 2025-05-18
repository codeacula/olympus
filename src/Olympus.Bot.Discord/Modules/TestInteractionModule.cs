using NetCord.Services.ApplicationCommands;
using Olympus.Application.Ai.Errors;
using Olympus.Application.Ai.Interactions.TalkWithGm;
using Olympus.Application.Common.Types;
using Olympus.Application.Grpc;

namespace Olympus.Bot.Discord.Modules;

public abstract partial class TestInteractionModule(
    IGrpcClient grpcClient,
    ILogger<TestInteractionModule> logger
  ) : BaseInteractionModule<TestInteractionModule>(grpcClient, logger)
{
  [SlashCommand("testinteraction", "Test Olympus")]
  public async Task<string> TestInteractionAsync(string interactionText)
  {
    var request = new TalkWithGmRequest(interactionText);
    var response = await GrpcClient.AiApiService.TalkWithGmAsync(request);

    return response switch
    {
      OlympusResult<TalkWithGmResponse, FailedToGetResponseError>.Success s
          => HandleSuccess<TalkWithGmResponse>(s.Value.Response),
      OlympusResult<TalkWithGmResponse, FailedToGetResponseError>.Failure f
          => HandleFailure(f.Error),
      _ => throw new InvalidOperationException("Unexpected response state.")
    };
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      EventId = 0,
      Message = "Test command executed with text: {InteractionText}")]
  public static partial void LogCommandExecuted(ILogger logger, string interactionText);
}
