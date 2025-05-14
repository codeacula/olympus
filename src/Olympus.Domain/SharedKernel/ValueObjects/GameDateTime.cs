namespace Olympus.Domain.SharedKernel.ValueObjects;

public readonly record struct GameDateTime(DateTime Value)
{
  public static GameDateTime From(DateTime dateTime) => new(dateTime);
  public static bool TryParse(string input, out GameDateTime gameDateTime)
  {
    if (DateTime.TryParse(input, out var dt))
    {
      gameDateTime = new GameDateTime(dt);
      return true;
    }
    gameDateTime = default;
    return false;
  }
  public override string ToString() => Value.ToString("O");
}
