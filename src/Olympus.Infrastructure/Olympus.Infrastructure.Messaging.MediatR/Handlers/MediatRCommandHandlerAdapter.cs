namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

internal sealed class MediatRCommandHandlerAdapter<TCommand, TResult>(IOlympusCommandHandler<TCommand, TResult> handler)
    : IRequestHandler<OlympusCommandToMediatRRequest<TCommand, TResult>, TResult>
    where TCommand : IOlympusCommand<TResult>
{
  private readonly IOlympusCommandHandler<TCommand, TResult> _handler = handler;

  public Task<TResult> Handle(OlympusCommandToMediatRRequest<TCommand, TResult> request, CancellationToken cancellationToken)
  {
    return _handler.HandleAsync(request.Command, cancellationToken);
  }
}
