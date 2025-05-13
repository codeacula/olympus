namespace Olympus.Application.Abstractions;

/// <summary>
/// Publishes domain or application events.
/// </summary>
public interface IEventPublisher
{
    ValueTask PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}
