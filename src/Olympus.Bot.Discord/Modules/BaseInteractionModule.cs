using NetCord.Services.ApplicationCommands;
using Olympus.Application.Common.Messaging;

namespace Olympus.Bot.Discord.Modules;

public abstract class BaseInteractionModule<TInteractionModuleType>(
  IOlympusDispatcher dispatcher,
  ILogger<TInteractionModuleType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TInteractionModuleType : BaseInteractionModule<TInteractionModuleType>
{
  protected IOlympusDispatcher Dispatcher { get; set; } = dispatcher;
  protected ILogger<TInteractionModuleType> Logger { get; set; } = logger;
}
