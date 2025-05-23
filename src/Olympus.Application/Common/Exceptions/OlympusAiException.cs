namespace Olympus.Application.Common.Exceptions;

public class OlympusAiException : Exception
{
  public OlympusAiException(string message) : base(message) { }

  public OlympusAiException()
  {
  }

  public OlympusAiException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
