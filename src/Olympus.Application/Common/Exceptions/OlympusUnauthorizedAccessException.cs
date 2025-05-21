namespace Olympus.Application.Common.Exceptions;

public class OlympusUnauthorizedAccessException : Exception
{
  public OlympusUnauthorizedAccessException(string message) : base(message) { }

  public OlympusUnauthorizedAccessException()
  {
  }

  public OlympusUnauthorizedAccessException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
