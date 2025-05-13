using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Olympus.Application.Ai;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;
using Olympus.Application.Common.Types;
using Xunit;

namespace Olympus.Tests.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;

public class ProcessPlayerNarrativeInputCommandHandlerTests
{
    private readonly ISemanticKernelOrchestrator _orchestrator;
    private readonly IGameSessionNarrativeContextService _contextService;
    private readonly ILogger<ProcessPlayerNarrativeInputCommandHandler> _logger;
    private readonly ProcessPlayerNarrativeInputCommandHandler _handler;

    public ProcessPlayerNarrativeInputCommandHandlerTests()
    {
        _orchestrator = Substitute.For<ISemanticKernelOrchestrator>();
        _contextService = Substitute.For<IGameSessionNarrativeContextService>();
        _logger = Substitute.For<ILogger<ProcessPlayerNarrativeInputCommandHandler>>();
        _handler = new ProcessPlayerNarrativeInputCommandHandler(_orchestrator, _contextService, _logger);
    }

    [Fact]
    public async Task Handle_GivenValidCommand_ShouldReturnFailNotImplemented()
    {
        // Arrange
        var command = new ProcessPlayerNarrativeInputCommand(
            SessionId: "session-123",
            PlayerId: "player-123",
            InputText: "I cast a fireball at the orc."
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Be("NotImplemented");
    }
}
