namespace Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

public readonly record struct EntityId(Guid Value)
{
  public static EntityId New() => new(Guid.NewGuid());
  public static bool TryParse(string input, out EntityId entityId)
  {
    if (Guid.TryParse(input, out var guid))
    {
      entityId = new EntityId(guid);
      return true;
    }
    entityId = default;
    return false;
  }
  public override string ToString() => Value.ToString();
}
