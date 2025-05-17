using NetCord.Services.ApplicationCommands;
using Olympus.Application.Common.Types;
using Olympus.Application.Grpc;

namespace Olympus.Bot.Discord.Modules;

public abstract class BaseInteractionModule<TInteractionModuleType>(
  IGrpcClient grpcClient,
  ILogger<TInteractionModuleType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TInteractionModuleType : BaseInteractionModule<TInteractionModuleType>
{
  protected IGrpcClient GrpcClient { get; set; } = grpcClient;
  protected ILogger<TInteractionModuleType> Logger { get; set; } = logger;

  protected string HandleFailure(OlympusError error)
  {
    ModuleLogger.LogCommandError(Logger, error.ToString());
    return $"An error occurred: {error.Message}";
  }

  protected string HandleSuccess<TResponseType>(string response)
  {
    ModuleLogger.LogCommandExecuted(Logger, nameof(TResponseType), response);
    return response;
  }
}
