namespace Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

public readonly record struct UserId(Guid Value)
{
  public static UserId New() => new(Guid.NewGuid());
  public static bool TryParse(string input, out UserId userId)
  {
    if (Guid.TryParse(input, out var guid))
    {
      userId = new UserId(guid);
      return true;
    }
    userId = default;
    return false;
  }
  public override string ToString() => Value.ToString();
}
