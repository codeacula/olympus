using FluentValidation;

namespace Olympus.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (_validators.Any())
    {
      var context = new ValidationContext<TRequest>(request);
      var validationResults = await Task.WhenAll(
          _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

      var failures = validationResults
          .SelectMany(r => r.Errors)
          .Where(f => f != null)
          .ToList();

      if (failures.Count != 0)
      {
        throw new OlympusValidationException("One or more validation failures have occurred.", failures);
      }
    }
    return await next(cancellationToken);
  }
}
