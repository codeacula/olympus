using FluentValidation;

namespace Olympus.Application.Common.Commands;

public class BaseHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : IRequest<TResult>
{
  private readonly IValidator<TCommand> _validator;

  public BaseHandler(IValidator<TCommand> validator)
  {
    _validator = validator;
  }

  public async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken)
  {
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
      throw new ValidationException(validationResult.Errors);
    }

    return await HandleInternal(request, cancellationToken);
  }

  protected virtual Task<TResult> HandleInternal(TCommand request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
