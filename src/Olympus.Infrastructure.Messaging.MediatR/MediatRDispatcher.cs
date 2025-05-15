using MediatR;
using Olympus.Application.Common.Messaging;

namespace Olympus.Infrastructure.Messaging.MediatR;

public class MediatRDispatcher(IMediator mediator) : IOlympusDispatcher
{
  private readonly IMediator _mediator = mediator;

  public Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
      where TRequest : class
  {
    return _mediator.Send(request, cancellationToken);
  }
}
