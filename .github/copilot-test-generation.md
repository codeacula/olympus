# Test Generation

## 1. Pattern

- **Arrange**: setup inputs, mocks.
- **Act**: invoke the unit.
- **Assert**: verify single expected outcome. One scenario per test.

## 2. Naming

`MethodUnderTest_GivenCondition_ShouldExpectedResult`

## 3. Example

    ```csharp
    [Fact]
    public async Task Handle_GivenValidInput_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new ProcessPlayerNarrativeInputCommandHandler(...);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    ```

## 4. Tools

- FluentAssertions
- NSubstitute or Moq
- AutoFixture for data
