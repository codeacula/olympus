namespace Olympus.Application.Common.Types;

public abstract record class OlympusOption<T>
{
  public sealed record class Some(T Value) : OlympusOption<T>;
  public sealed record class None : OlympusOption<T>;
}
