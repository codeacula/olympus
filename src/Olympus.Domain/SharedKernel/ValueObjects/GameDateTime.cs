namespace Olympus.Domain.SharedKernel.ValueObjects;

public readonly record struct GameDateTime
{
  public DateTime Value { get; }

  public GameDateTime(DateTime value)
  {
    // Ensure UTC
    Value = value.Kind == DateTimeKind.Utc
        ? value
        : DateTime.SpecifyKind(value, DateTimeKind.Utc);
  }

  public static GameDateTime From(DateTime dateTime) => new(dateTime);
  public static bool TryParse(string input, out GameDateTime gameDateTime)
  {
    if (DateTime.TryParse(input, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal, out var dt))
    {
      // Ensure UTC
      dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
      gameDateTime = new GameDateTime(dt);
      return true;
    }
    gameDateTime = default;
    return false;
  }

  public override string ToString() => Value.ToString("O");
}
