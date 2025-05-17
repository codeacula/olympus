namespace Olympus.Domain.Events;

public interface IDomainEvent
{
  DateTimeOffset OccurredOn { get; }
}
