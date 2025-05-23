using NetCord.Services.ApplicationCommands;
using Olympus.Application.Grpc;
using Olympus.Bot.Discord.Exceptions;

namespace Olympus.Bot.Discord.Modules;

public abstract partial class BaseInteractionModule<TInteractionModuleType>(
  IGrpcClient grpcClient,
  ILogger<TInteractionModuleType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TInteractionModuleType : BaseInteractionModule<TInteractionModuleType>
{
  protected IGrpcClient GrpcClient { get; set; } = grpcClient;
  protected ILogger<TInteractionModuleType> Logger { get; set; } = logger;

  protected async Task<string> ExecuteAsync<TRequestType, TResponseType>(Func<Task<string>> action)
  {
    try
    {
      LogExecutingCommand(Logger, nameof(TRequestType));
      var response = (await action()) ?? throw new DiscordResponseNullException();
      LogCommandExecuted(Logger, typeof(TResponseType), response ?? string.Empty);

      return response!;
    }
    catch (Exception ex)
    {
      LogCommandFailure(Logger, typeof(TRequestType), ex);
      throw;
    }
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Executing command {CommandName}")]
  public static partial void LogExecutingCommand(ILogger<TInteractionModuleType> logger, string commandName);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Command {CommandExecuted} executed with response: {Response}")]
  public static partial void LogCommandExecuted(ILogger<TInteractionModuleType> logger, Type commandExecuted, string response);

  [LoggerMessage(
      Level = LogLevel.Error,
      Message = "Command {CommandName} failed")]
  public static partial void LogCommandFailure(ILogger<TInteractionModuleType> logger, Type commandName, Exception ex);
}
