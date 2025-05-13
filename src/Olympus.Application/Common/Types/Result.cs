namespace Olympus.Application.Common.Types;

/// <summary>
/// Represents the result of an operation, with success or error.
/// </summary>
public abstract record class Result<TSuccess, TError>
{
    public sealed record class Success(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record class Failure(TError Error) : Result<TSuccess, TError>;

    public static Success Ok(TSuccess value) => new(value);
    public static Failure Fail(TError error) => new(error);
}
