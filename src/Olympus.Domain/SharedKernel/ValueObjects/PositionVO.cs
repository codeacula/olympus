namespace Olympus.Domain.SharedKernel.ValueObjects;

public readonly record struct PositionVO(int X, int Y)
{
  public override string ToString() => $"({X}, {Y})";
  public static PositionVO From(int x, int y) => new(x, y);
  public static bool TryParse(string input, out PositionVO position)
  {
    position = default;
    if (string.IsNullOrWhiteSpace(input))
    {
      return false;
    }

    var parts = input.Trim('(', ')').Split(',');
    if (parts.Length == 2 && int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
    {
      position = new PositionVO(x, y);
      return true;
    }
    return false;
  }
}
