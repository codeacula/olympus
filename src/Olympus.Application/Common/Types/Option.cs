namespace Olympus.Application.Common.Types;

/// <summary>
/// Represents an optional value.
/// </summary>
public abstract record class Option<T>
{
    public sealed record class Some(T Value) : Option<T>;
    public sealed record class None : Option<T>;

    public static Some SomeValue(T value) => new(value);
    public static None NoValue() => new();
}
