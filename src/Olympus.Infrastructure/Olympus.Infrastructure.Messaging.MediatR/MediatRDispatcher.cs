namespace Olympus.Infrastructure.Messaging.MediatR;

public class MediatRDispatcher(IMediator mediator) : IOlympusDispatcher
{
  private readonly IMediator _mediator = mediator;

  public async Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : class
  {
    ArgumentNullException.ThrowIfNull(request);

    var response = await _mediator.Send(request, cancellationToken);
    return response is null
      ? throw new InvalidOperationException($"The mediator returned a null response for request of type {typeof(TRequest).Name}.")
      : (TResponse)response;
  }

  public Task<TResult> DispatchCommandAsync<TResult>(IOlympusCommand<TResult> command, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<TResult> DispatchQueryAsync<TResult>(IOlympusQuery<TResult> query, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
