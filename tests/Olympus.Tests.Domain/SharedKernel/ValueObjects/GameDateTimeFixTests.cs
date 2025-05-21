using Olympus.Domain.SharedKernel.ValueObjects;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class GameDateTimeFixTests
{
  [Fact]
  public void Constructor_InitializesWithCorrectValue()
  {
    // Arrange
    var expectedDate = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    // Act
    var gameDateTime = new GameDateTime(expectedDate);

    // Assert
    Assert.Equal(expectedDate, gameDateTime.Value);
  }

  [Fact]
  public void FromMethod_CreatesInstanceWithCorrectValue()
  {
    // Arrange
    var expectedDate = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    // Act
    var gameDateTime = GameDateTime.From(expectedDate);

    // Assert
    Assert.Equal(expectedDate, gameDateTime.Value);
  }
}
