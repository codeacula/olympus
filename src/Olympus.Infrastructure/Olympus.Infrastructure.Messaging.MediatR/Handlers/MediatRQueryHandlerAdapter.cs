namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

internal sealed class MediatRQueryHandlerAdapter<TQuery, TResult>(IOlympusQueryHandler<TQuery, TResult> handler)
    : IRequestHandler<OlympusQueryToMediatRRequest<TQuery, TResult>, TResult>
    where TQuery : IOlympusQuery<TResult>
{
  private readonly IOlympusQueryHandler<TQuery, TResult> _handler = handler;

  public Task<TResult> Handle(OlympusQueryToMediatRRequest<TQuery, TResult> request, CancellationToken cancellationToken)
  {
    return _handler.HandleAsync(request.Query, cancellationToken);
  }
}
