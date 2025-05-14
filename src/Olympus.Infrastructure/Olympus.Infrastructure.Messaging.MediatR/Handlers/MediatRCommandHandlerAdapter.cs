using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

/// <summary>
/// Adapts the Olympus command handler interface to MediatR's request handler
/// </summary>
/// <param name="handler"></param>
internal class MediatRCommandHandlerAdapter<TCommand, TResult>(IOlympusCommandHandler<TCommand, TResult> handler)
    : IRequestHandler<OlympusCommandToMediatRRequest<TCommand, TResult>, TResult>
    where TCommand : IOlympusCommand<TResult>
{
  private readonly IOlympusCommandHandler<TCommand, TResult> _handler = handler;

  public Task<TResult> Handle(OlympusCommandToMediatRRequest<TCommand, TResult> request, CancellationToken cancellationToken)
  {
    return _handler.HandleAsync(request.Command, cancellationToken);
  }
}
