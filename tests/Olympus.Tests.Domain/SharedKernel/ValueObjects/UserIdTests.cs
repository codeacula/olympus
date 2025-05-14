using Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class UserIdTests
{
  [Fact]
  public void Equality_WithSameGuid_ShouldBeEqual()
  {
    var guid = Guid.NewGuid();
    var id1 = new UserId(guid);
    var id2 = new UserId(guid);
    Assert.Equal(id1, id2);
  }

  [Fact]
  public void ToString_And_Parse_RoundTrip()
  {
    var original = UserId.New();
    var str = original.ToString();
    Assert.True(UserId.TryParse(str, out var parsed));
    Assert.Equal(original, parsed);
  }
}
