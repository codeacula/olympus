namespace Olympus.Domain.Errors;

public class DomainError : Exception
{
  public DomainError()
  {
  }

  public DomainError(string message, Exception innerException) : base(message, innerException)
  {
  }

  public DomainError(string message) : base(message)
  {
  }
}
