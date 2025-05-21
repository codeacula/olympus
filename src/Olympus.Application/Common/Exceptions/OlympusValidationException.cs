using FluentValidation.Results;

namespace Olympus.Application.Common.Exceptions;

public class OlympusValidationException : Exception
{
  public IReadOnlyDictionary<string, string[]> Errors { get; }

  public OlympusValidationException(string message, IEnumerable<ValidationFailure> failures)
      : base(message)
  {
    Errors = failures
        .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
        .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
  }

  public OlympusValidationException(string message) : base(message)
  {
    Errors = new Dictionary<string, string[]>();
  }

  public OlympusValidationException()
  {
    Errors = new Dictionary<string, string[]>();
  }

  public OlympusValidationException(string? message, Exception? innerException) : base(message, innerException)
  {
    Errors = new Dictionary<string, string[]>();
  }
}
