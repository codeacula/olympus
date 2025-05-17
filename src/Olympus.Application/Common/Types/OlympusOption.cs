namespace Olympus.Application.Common.Types;

public abstract record class OlympusOption<T>
{
  public sealed record class Some(T Value) : OlympusOption<T>;
  public sealed record class None : OlympusOption<T>;
  public static Some SomeValue(T value) => new(value);
  public static None NoValue() => new();
}
