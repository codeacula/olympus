using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR.Handlers;

/// <summary>
/// Adapts the Olympus event handler interface to MediatR's notification handler
/// </summary>
internal class MediatREventHandlerAdapter<TEvent>
    : INotificationHandler<OlympusEventToMediatRNotification<TEvent>>
    where TEvent : IOlympusEvent
{
  private readonly IOlympusEventHandler<TEvent> _handler;

  public MediatREventHandlerAdapter(IOlympusEventHandler<TEvent> handler)
  {
    _handler = handler;
  }

  public Task Handle(OlympusEventToMediatRNotification<TEvent> notification, CancellationToken cancellationToken)
  {
    return _handler.HandleAsync(notification.Event, cancellationToken);
  }
}
