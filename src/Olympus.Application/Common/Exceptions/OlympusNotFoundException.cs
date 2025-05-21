namespace Olympus.Application.Common.Exceptions;

public class OlympusNotFoundException : Exception
{
  public OlympusNotFoundException(string resourceName, object key)
      : base($"Resource \"{resourceName}\" ({key}) was not found.") { }

  public OlympusNotFoundException(string message) : base(message) { }

  public OlympusNotFoundException()
  {
  }

  public OlympusNotFoundException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
