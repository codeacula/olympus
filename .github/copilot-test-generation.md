---
applyTo: "**/*Tests.cs" # Applies when generating tests
---

# Olympus Test Generation Guidelines (xUnit)

When generating unit tests for the Olympus project using xUnit, please adhere to the following:

**1. Test Structure (AAA Pattern):**

* Clearly structure each test method using the Arrange, Act, and Assert (AAA) pattern.
* Use comments (`// Arrange`, `// Act`, `// Assert`) or blank lines to visually separate
    these sections.

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

* Test methods should be clearly named: `MethodUnderTest_Scenario_ExpectedOutcome`.
* Test classes should be named after the class they are testing, suffixed with `Tests`
    (e.g., `CharacterServiceTests`).

**3. Single Responsibility per Test:**

* Each test method should verify only one specific behavior or outcome.
* If a method has multiple distinct outcomes, create separate test methods for each.

**4. Assertions:**

* Use xUnit's assertion library (`Xunit.Assert`).
* Make assertions specific and clear (e.g., `Assert.Equal(expected, result)`).
* When asserting on collections, use collection-specific assertions (e.g., `Assert.Contains`).
* For `Result<TSuccess, TError>` types, assert `IsSuccess` or `IsFailure`, then the `Value` or `Error`.

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

* Use a mocking framework like Moq (or NSubstitute if preferred project-wide).
* Mock dependencies of the System Under Test (SUT).
* Verify mock interactions if they are part of the behavior being tested.

**6. Test Data:**

* Use clear and representative test data. Assign to clearly named variables.
* Avoid "magic values".

**7. Independence and Repeatability:**

* Tests must be independent and repeatable.
* Avoid dependencies on external systems for unit tests.

**8. Coverage:**

* Aim for good coverage of logic, edge cases, and boundary conditions.
* For `Result<TSuccess, TError>` types, test both success and failure paths.

**9. Readability:**

* Tests should be as readable as production code and serve as documentation.

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
            .Returns(Task.CompletedTask);
        _mockEventPublisher.Setup(p => p.PublishAsync(It.IsAny<CharacterCreatedEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
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
        Assert.IsType<ApplicationError>(result.Error);
        Assert.Contains("Database error", result.Error.Message);
    }
}
```

Focus on testing the logic within the unit, mocking its external dependencies effectively.
