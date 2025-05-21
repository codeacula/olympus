namespace Olympus.Application.Common.Exceptions;

public class OlympusInvalidResponseException : Exception
{
  public OlympusInvalidResponseException(string message) : base(message) { }

  public OlympusInvalidResponseException()
  {
  }

  public OlympusInvalidResponseException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
