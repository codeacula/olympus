---

applyTo: "**/*Tests.cs" # Applies when generating tests, assuming a common naming convention
---

## Olympus Test Generation Guidelines (xUnit)

When generating unit tests for the Olympus project using xUnit, please adhere to the following:

**1. Test Structure (AAA Pattern):**
    *Clearly structure each test method using the Arrange, Act, and Assert (AAA) pattern.
    * Use comments (`// Arrange`, `// Act`, `// Assert`) or blank lines to visually separate these sections.

    ```csharp
    [Fact]
    public void MethodUnderTest_Scenario_ExpectedOutcome()
    {
        // Arrange
        // Setup mocks, create test data, instantiate the SUT (System Under Test)

        // Act
        // Execute the method being tested

        // Assert
        // Verify the outcome, check mock interactions, assert on state changes or returned values
    }
    ```

**2. Naming Conventions:**
    *Test methods should be clearly named to indicate what they are testing: `MethodUnderTest_Scenario_ExpectedOutcome`.
    * Test classes should be named after the class they are testing, suffixed with `Tests` (e.g., `CharacterServiceTests`).

**3. Single Responsibility per Test:**
    *Each test method should verify only one specific behavior or outcome. Avoid multiple unrelated assertions in a single test.
    * If a method has multiple distinct outcomes based on different inputs or states, create separate test methods for each.

**4. Assertions:**
    *Use xUnit's assertion library (`Xunit.Assert`).
    * Make assertions specific and clear. For example, instead of `Assert.True(result == expected)`, use `Assert.Equal(expected, result)`.
    *When asserting on collections, use collection-specific assertions where appropriate (e.g., `Assert.Contains`, `Assert.All`).
    * For `Result<TSuccess, TError>` types, assert on `IsSuccess` or `IsFailure` first, then assert on the `Value` or `Error` accordingly.
        ```csharp
        // Example for Result<T,E>
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedValue, result.Value);
        // or
        Assert.True(result.IsFailure);
        Assert.IsType<SpecificDomainError>(result.Error);
        Assert.Equal("Expected error message", result.Error.Message);
        ```

**5. Mocking and Stubbing:**
    *Use a mocking framework like Moq or NSubstitute (check project dependencies to see which one, if any, is standard). For now, assume Moq if one needs to be chosen.
    * Mock dependencies of the System Under Test (SUT).
    *Only mock types you own or have control over where feasible. Avoid mocking concrete types from external libraries if their interfaces are available.
    * Verify mock interactions if they are part of the behavior being tested (e.g., `mockRepository.Verify(r => r.AddAsync(It.IsAny<Character>()), Times.Once);`).

**6. Test Data:**
    *Use clear and representative test data.
    * Avoid "magic values"; assign test data to clearly named variables.
    * Consider using libraries like AutoFixture for generating test data if it's a project standard, but simple manual creation is fine for most cases.

**7. Independence and Repeatability:**
    *Tests must be independent of each other. The outcome of one test should not affect another.
    * Tests must be repeatable and produce the same results every time they are run. Avoid dependencies on external systems (unless writing integration tests, which have different considerations).

**8. Coverage:**
    *Aim for good test coverage of the logic being implemented. Test edge cases, boundary conditions, and common scenarios.
    * For `Result<TSuccess, TError>` types, ensure both success and failure paths are tested.

**9. Readability:**
    * Tests should be as readable as the production code. They serve as documentation for the behavior of the code under test.

**Example for a Command Handler Test:**

```csharp
public class CreateCharacterCommandHandlerTests
{
    private readonly Mock<ICharacterRepository> _mockCharacterRepository;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly CreateCharacterCommandHandler _handler;

    public CreateCharacterCommandHandlerTests()
    {
        _mockCharacterRepository = new Mock<ICharacterRepository>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new CreateCharacterCommandHandler(_mockCharacterRepository.Object, _mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessAndCreateCharacter()
    {
        // Arrange
        var command = new CreateCharacterCommand("Test Character", CharacterClass.Fighter);
        var cancellationToken = CancellationToken.None;

        _mockCharacterRepository.Setup(r => r.AddAsync(It.IsAny<Character>(), cancellationToken))
            .Returns(Task.CompletedTask); // Or returns the character if AddAsync does
        _mockEventPublisher.Setup(p => p.PublishAsync(It.IsAny<CharacterCreatedEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        // Assuming Success is a simple type or you want to check the CharacterId from result.Value
        // Assert.NotNull(result.Value.CharacterId); 
        _mockCharacterRepository.Verify(r => r.AddAsync(It.Is<Character>(c => c.Name == command.Name), cancellationToken), Times.Once);
        _mockEventPublisher.Verify(p => p.PublishAsync(It.Is<CharacterCreatedEvent>(e => e.Name == command.Name), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateCharacterCommand("Test Character", CharacterClass.Fighter);
        var cancellationToken = CancellationToken.None;
        var expectedException = new InvalidOperationException("Database error");

        _mockCharacterRepository.Setup(r => r.AddAsync(It.IsAny<Character>(), cancellationToken))
            .ThrowsAsync(expectedException);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<ApplicationError>(result.Error); // Assuming a generic ApplicationError for such cases
        Assert.Contains("Database error", result.Error.Message);
    }
}
