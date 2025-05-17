namespace Olympus.Infrastructure.Messaging.MediatR.Adapters;

/// <summary>
/// Adapter to convert Olympus events to MediatR notifications
/// </summary>
/// <typeparam name="TEvent">The Olympus event type</typeparam>
/// <remarks>
/// Creates a new adapter wrapping the specified event
/// </remarks>
/// <param name="event">The event to wrap</param>
internal sealed class OlympusEventToMediatRNotification<TEvent>(TEvent @event) : INotification
    where TEvent : IOlympusEvent
{
  /// <summary>
  /// The wrapped Olympus event
  /// </summary>
  public TEvent Event { get; } = @event;
}
