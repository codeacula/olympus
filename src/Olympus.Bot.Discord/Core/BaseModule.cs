using NetCord.Services.ApplicationCommands;

namespace Olympus.Bot.Discord.Core;

public abstract class BaseModule<TType>(ILogger<TType> logger) : ApplicationCommandModule<ApplicationCommandContext>
  where TType : class
{
  protected ILogger<TType> Logger => logger;
}
