using Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class SessionIdTests
{
  [Fact]
  public void Equality_WithSameGuid_ShouldBeEqual()
  {
    var guid = Guid.NewGuid();
    var id1 = new SessionId(guid);
    var id2 = new SessionId(guid);
    Assert.Equal(id1, id2);
  }

  [Fact]
  public void ToString_And_Parse_RoundTrip()
  {
    var original = SessionId.New();
    var str = original.ToString();
    Assert.True(SessionId.TryParse(str, out var parsed));
    Assert.Equal(original, parsed);
  }
}
