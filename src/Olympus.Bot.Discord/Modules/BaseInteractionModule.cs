using NetCord.Services.ApplicationCommands;
using Olympus.Application.Common.Grpc;

namespace Olympus.Bot.Discord.Modules;

public abstract class BaseInteractionModule<TInteractionModuleType>(
  IGrpcClient grpcClient,
  ILogger<TInteractionModuleType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TInteractionModuleType : BaseInteractionModule<TInteractionModuleType>
{
  protected IGrpcClient GrpcClient { get; set; } = grpcClient;
  protected ILogger<TInteractionModuleType> Logger { get; set; } = logger;

  protected string HandleFailure(string errorMsg)
  {
    ModuleLogger.LogCommandFailure(Logger, errorMsg);
    return $"An error occurred: {errorMsg}";
  }

  protected string HandleSuccess<TResponseType>(string response)
  {
    ModuleLogger.LogCommandExecuted(Logger, nameof(TResponseType), response);
    return response;
  }
}
