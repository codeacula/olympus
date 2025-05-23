namespace Olympus.Bot.Discord.Exceptions;

public class DiscordResponseNullException : Exception
{
  public DiscordResponseNullException()
  {
  }
  public DiscordResponseNullException(string message) : base(message)
  {
  }

  public DiscordResponseNullException(string message, Exception innerException) : base(message, innerException)
  {
  }
}
