using NetCord.Services.ApplicationCommands;
using Olympus.Application.Grpc;

namespace Olympus.Bot.Discord.Modules;

public abstract partial class BaseInteractionModule<TInteractionModuleType>(
  IGrpcClient grpcClient,
  ILogger<TInteractionModuleType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TInteractionModuleType : BaseInteractionModule<TInteractionModuleType>
{
  protected IGrpcClient GrpcClient { get; set; } = grpcClient;
  protected ILogger<TInteractionModuleType> Logger { get; set; } = logger;

  protected async Task<TResponseType> ExecuteAsync<TRequestType, TResponseType>(Func<Task<TResponseType>> action)
  {
    try
    {
      LogExecutingCommand(Logger, nameof(TRequestType));
      var response = await action();

      if (response is null)
      {
        HandleFailure("Unknown error occurred");
        return default!;
      }

      return HandleSuccess<TResponseType>(response.ToString());
    }
    catch (Exception ex)
    {
      HandleFailure(ex.Message);
      throw;
    }
  }

  protected string HandleFailure(string errorMsg)
  {
    ModuleLogger.LogCommandFailure(Logger, errorMsg);
    return $"An error occurred: {errorMsg}";
  }

  protected TResponseType HandleSuccess<TResponseType>(TResponseType response)
  {
    LogCommandExecuted(Logger, typeof(TResponseType), response!.ToString() ?? string.Empty);
    return response;
  }

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Executing command {CommandName}")]
  public static partial void LogExecutingCommand(ILogger<TInteractionModuleType> logger, string commandName);

  [LoggerMessage(
      Level = LogLevel.Information,
      Message = "Command {CommandExecuted} executed with response: {Response}")]
  public static partial void LogCommandExecuted(ILogger<TInteractionModuleType> logger, Type commandExecuted, string response);
}
