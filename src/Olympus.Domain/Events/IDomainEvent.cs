namespace Olympus.Domain.Events;

public interface IDomainEvent
{
  DateTime OccurredOn { get; }
}
