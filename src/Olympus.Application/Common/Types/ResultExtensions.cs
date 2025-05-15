namespace Olympus.Application.Common.Types;

/// <summary>
/// Extension methods for working with Result objects.
/// </summary>
public static class ResultExtensions
{
  /// <summary>
  /// Match the result to different actions based on whether it's a success or failure.
  /// </summary>
  /// <typeparam name="TSuccess">The success value type</typeparam>
  /// <typeparam name="TError">The error value type</typeparam>
  /// <typeparam name="TResult">The result type of the match operation</typeparam>
  /// <param name="result">The result to match on</param>
  /// <param name="onSuccess">Function to execute for Success case</param>
  /// <param name="onFailure">Function to execute for Failure case</param>
  /// <returns>The result of either the success or failure function</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static TResult Match<TSuccess, TError, TResult>(
      this Result<TSuccess, TError> result,
      Func<TSuccess, TResult> onSuccess,
      Func<TError, TResult> onFailure)
  {
    return result switch
    {
      Result<TSuccess, TError>.Success success => onSuccess(success.Value),
      Result<TSuccess, TError>.Failure failure => onFailure(failure.Error),
      _ => throw new InvalidOperationException($"Unknown result type: {result.GetType().Name}")
    };
  }
}
