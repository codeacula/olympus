using MediatR;
using Olympus.Application.Common.Messaging;

namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus events to MediatR notifications
/// </summary>
/// <typeparam name="TEvent">The Olympus event type</typeparam>
internal class OlympusEventToMediatRNotification<TEvent> : INotification
    where TEvent : IOlympusEvent
{
    /// <summary>
    /// The wrapped Olympus event
    /// </summary>
    public TEvent Event { get; }

    /// <summary>
    /// Creates a new adapter wrapping the specified event
    /// </summary>
    /// <param name="event">The event to wrap</param>
    public OlympusEventToMediatRNotification(TEvent @event)
    {
        Event = @event;
    }
}
