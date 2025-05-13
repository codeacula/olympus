namespace Olympus.Domain.Errors;

public class DomainError(string message) : Exception(message)
{ }
