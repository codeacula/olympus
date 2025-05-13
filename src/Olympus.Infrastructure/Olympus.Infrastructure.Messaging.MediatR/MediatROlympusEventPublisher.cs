using MediatR;
using Olympus.Application.Common.Messaging;
using Olympus.Infrastructure.Messaging.MediatR.Adapters;

namespace Olympus.Infrastructure.Messaging.MediatR;

/// <summary>
/// Implementation of the Olympus event publisher using MediatR
/// </summary>
public class MediatROlympusEventPublisher : IOlympusEventPublisher
{
    private readonly IPublisher _publisher;

    public MediatROlympusEventPublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IOlympusEvent
    {
        var notification = new OlympusEventToMediatRNotification<TEvent>(@event);
        await _publisher.Publish(notification, cancellationToken);
    }
}
