using Olympus.Application.Common.Types;

namespace Olympus.Tests.Application;

public class ResultOptionTests
{
  [Fact]
  public void Result_Ok_ShouldContainValue()
  {
    var result = Result<int, string>.Ok(42);
    _ = Assert.IsType<Result<int, string>.Success>(result);
    Assert.Equal(42, result.Value);
  }

  [Fact]
  public void Result_Fail_ShouldContainError()
  {
    var result = Result<int, string>.Fail("error");
    _ = Assert.IsType<Result<int, string>.Failure>(result);
    Assert.Equal("error", result.Error);
  }

  [Fact]
  public void Option_Some_ShouldContainValue()
  {
    var option = OlympusOption<int>.SomeValue(99);
    _ = Assert.IsType<OlympusOption<int>.Some>(option);
    Assert.Equal(99, option.Value);
  }

  [Fact]
  public void Option_None_ShouldBeNone()
  {
    var option = OlympusOption<int>.NoValue();
    _ = Assert.IsType<OlympusOption<int>.None>(option);
  }
}
