namespace Olympus.Bot.Discord.Core;

public class DiscordSettings
{
  public required string ApplicationId { get; set; }
  public required string BotToken { get; set; }

  public required string ClientId { get; set; }

  public required string ClientSecret { get; set; }

  public required string PublicKey { get; set; }
}
