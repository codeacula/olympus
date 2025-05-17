namespace Olympus.Application.Common.Types;

public abstract record class OlympusResult<TSuccess, TError>
  where TSuccess : notnull
  where TError : OlympusError
{
  public sealed record class Success(TSuccess Value) : OlympusResult<TSuccess, TError>;
  public sealed record class Failure(TError Error) : OlympusResult<TSuccess, TError>;
}
