namespace Olympus.Application.Common.Types;

public interface IOlympusResult
{
  bool IsSuccess { get; }
  bool IsFailure => !IsSuccess;
}
