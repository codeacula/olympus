namespace Olympus.Application.Common.Exceptions;

public class OlympusDomainRuleException : Exception
{
  public OlympusDomainRuleException(string message) : base(message) { }

  public OlympusDomainRuleException()
  {
  }

  public OlympusDomainRuleException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}
