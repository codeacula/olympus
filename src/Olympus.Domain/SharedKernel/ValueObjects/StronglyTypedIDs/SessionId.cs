namespace Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

public readonly record struct SessionId(Guid Value)
{
  public static SessionId New() => new(Guid.NewGuid());
  public static bool TryParse(string input, out SessionId sessionId)
  {
    if (Guid.TryParse(input, out var guid))
    {
      sessionId = new SessionId(guid);
      return true;
    }
    sessionId = default;
    return false;
  }
  public override string ToString() => Value.ToString();
}
