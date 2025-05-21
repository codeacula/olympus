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
    // Use a specific datetime with UTC kind
    var now = DateTime.SpecifyKind(new DateTime(2023, 5, 15, 10, 30, 0), DateTimeKind.Utc);
    var original = new GameDateTime(now);
    var str = original.ToString();
    Assert.True(GameDateTime.TryParse(str, out var parsed));

    // Compare the underlying ticks which are timezone-independent
    Assert.Equal(original.Value.Ticks, parsed.Value.Ticks);
  }
}
