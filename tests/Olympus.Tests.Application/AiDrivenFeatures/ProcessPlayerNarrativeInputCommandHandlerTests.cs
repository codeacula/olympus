using Moq;
using Olympus.Application.Ai;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;
using Olympus.Application.Common.Types;
using Olympus.Domain.SharedKernel.ValueObjects;

namespace Olympus.Tests.Application.AiDrivenFeatures;

public class ProcessPlayerNarrativeInputCommandHandlerTests
{
  private readonly Mock<ISemanticKernelOrchestrator> _semanticKernelOrchestratorMock;
  private readonly Mock<IGameSessionNarrativeContextService> _narrativeContextServiceMock;
  private readonly ProcessPlayerNarrativeInputCommandHandler _handler;

  public ProcessPlayerNarrativeInputCommandHandlerTests()
  {
    // Setup mocks
    _semanticKernelOrchestratorMock = new Mock<ISemanticKernelOrchestrator>();
    _narrativeContextServiceMock = new Mock<IGameSessionNarrativeContextService>();

    // Setup test context
    const string sessionId = "test-session";
    _ = _narrativeContextServiceMock
        .Setup(x => x.GetContextAsync(sessionId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Option<NarrativeContext>.NoValue());

    // Create handler with mocked dependencies
    _handler = new ProcessPlayerNarrativeInputCommandHandler(
        _semanticKernelOrchestratorMock.Object,
        _narrativeContextServiceMock.Object,
        Mock.Of<Microsoft.Extensions.Logging.ILogger<ProcessPlayerNarrativeInputCommandHandler>>());
  }

  [Fact]
  public async Task HandleAsync_WithValidInput_ReturnsSuccessfulResponseAsync()
  {
    // Arrange
    var command = new ProcessPlayerNarrativeInputCommand(
        new SessionId("test-session"),
        new UserId("test-user"),
        "This is a test input");

    // Act
    var result = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert
    Assert.True(result is Result<NarrativeResponseDto, Error>.Success);
    var successResult = (Result<NarrativeResponseDto, Error>.Success)result;
    Assert.NotNull(successResult.Value);
    Assert.NotNull(successResult.Value.Response);
    Assert.Contains(command.Input, successResult.Value.Response);
    Assert.NotEmpty(successResult.Value.UpdatedContext);
  }
}
