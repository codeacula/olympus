using Olympus.Domain.SharedKernel.ValueObjects.StronglyTypedIDs;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class EntityIdTests
{
  [Fact]
  public void Equality_WithSameGuid_ShouldBeEqual()
  {
    var guid = Guid.NewGuid();
    var id1 = new EntityId(guid);
    var id2 = new EntityId(guid);
    Assert.Equal(id1, id2);
  }

  [Fact]
  public void ToString_And_Parse_RoundTrip()
  {
    var original = EntityId.New();
    var str = original.ToString();
    Assert.True(EntityId.TryParse(str, out var parsed));
    Assert.Equal(original, parsed);
  }
}
