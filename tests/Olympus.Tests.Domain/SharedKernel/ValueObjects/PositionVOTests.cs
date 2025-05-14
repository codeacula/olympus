using Olympus.Domain.SharedKernel.ValueObjects;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class PositionVOTests
{
  [Fact]
  public void Equality_WithSameCoordinates_ShouldBeEqual()
  {
    var pos1 = new PositionVO(1, 2);
    var pos2 = new PositionVO(1, 2);
    Assert.Equal(pos1, pos2);
  }

  [Fact]
  public void ToString_And_Parse_RoundTrip()
  {
    var original = new PositionVO(5, 7);
    var str = original.ToString();
    Assert.True(PositionVO.TryParse(str, out var parsed));
    Assert.Equal(original, parsed);
  }
}
