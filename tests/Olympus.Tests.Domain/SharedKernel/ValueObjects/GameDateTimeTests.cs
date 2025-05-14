using Olympus.Domain.SharedKernel.ValueObjects;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class GameDateTimeTests
{
  [Fact]
  public void Equality_WithSameDateTime_ShouldBeEqual()
  {
    var dt = DateTime.UtcNow;
    var gdt1 = new GameDateTime(dt);
    var gdt2 = new GameDateTime(dt);
    Assert.Equal(gdt1, gdt2);
  }

  [Fact]
  public void ToString_And_Parse_RoundTrip()
  {
    var now = DateTime.UtcNow;
    var original = new GameDateTime(now);
    var str = original.ToString();
    Assert.True(GameDateTime.TryParse(str, out var parsed));
    Assert.Equal(original.Value.ToString("O"), parsed.Value.ToString("O"));
  }
}
