namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

internal sealed class MediatREventHandlerAdapter<TEvent>(IOlympusEventHandler<TEvent> handler)
    : INotificationHandler<OlympusEventToMediatRNotification<TEvent>>
    where TEvent : IOlympusEvent
{
  private readonly IOlympusEventHandler<TEvent> _handler = handler;

  public Task Handle(OlympusEventToMediatRNotification<TEvent> notification, CancellationToken cancellationToken)
  {
    return _handler.HandleAsync(notification.Event, cancellationToken);
  }
}
