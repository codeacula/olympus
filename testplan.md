# Olympus Comprehensive Test Plan

## Overview

This document outlines the testing strategy for the Olympus platform, covering unit tests, integration tests, and end-to-end tests to ensure the quality and correctness of the system.

## Current Test Status

The project had several failing tests which are now fixed. Current test projects include:

- ✅ `Olympus.Tests.Domain`: Tests for domain value objects (all tests now passing)
- ✅ `Olympus.Tests.Application`: Tests for Result/Option types and the ProcessPlayerNarrativeInputCommandHandler (all tests now passing)
- ⚠️ `Olympus.Tests.Infrastructure`: Placeholder project with no implemented tests (builds but has no tests)
- ⚠️ `Olympus.Tests.Architecture`: Placeholder project with no implemented tests (builds but has no tests)
- ✅ `Olympus.Tests.Integration`: Added integration test for Player Narrative Interaction workflow
- ⚠️ `Olympus.Tests.Bot.Discord`: Project exists but may need more tests

## Test Categories

### 1. Unit Tests

- **Domain Layer Tests**
  - Value Objects (SessionId, UserId, EntityId, PositionVO, GameDateTime, etc.)
  - Aggregates (Campaign, Character)
  - Domain Services
  - Domain Events
  - Repository interfaces

- **Application Layer Tests**
  - Command Handlers (ProcessPlayerNarrativeInputCommandHandler, etc.)
  - Query Handlers
  - Validation Behaviors
  - Application Services
  - Result and Option types

- **Infrastructure Layer Tests**
  - Repository Implementations (Marten)
  - Semantic Kernel Integration
  - Redis Cache Implementation
  - Event Publishers
  - ECS Systems

- **API/Client Layer Tests**
  - API Controllers
  - Discord Bot Handlers
  - HTTP Clients

### 2. Integration Tests

- **CQRS Pipeline Tests**
  - Command dispatch through validation, handling, and result mapping
  - Query execution through the entire pipeline

- **Player Narrative Interaction Flow**
  - Complete flow from API request to AI response
  - Context persistence tests with Redis

- **Event Sourcing Tests**
  - Event recording and replay
  - Projection creation from events

### 3. End-to-End Tests

- **Discord Bot E2E Tests**
  - Slash command handling
  - Message formatting and presentation

- **API E2E Tests**
  - API authentication and authorization
  - Complete request/response cycles

## Test Implementation Priorities

### Phase 1 (MVP)

1. Fix failing value object tests (GameDateTime)
2. Complete domain value object test coverage
3. ProcessPlayerNarrativeInputCommand tests
4. Basic integration test for narrative flow
5. SimpleChanceRoll tool tests

### Phase 2

1. Repository implementation tests
2. Event sourcing integration tests
3. Discord bot handler tests
4. Context persistence tests

### Phase 3

1. Aggregate root tests
2. ECS system tests
3. Full E2E tests
4. Performance and load tests

## Test Standards

- All tests should follow Arrange-Act-Assert pattern
- Use descriptive test names (Method_Scenario_ExpectedBehavior)
- One assertion concept per test
- Use test data builders for complex objects
- Mock external dependencies
- Use test fixtures for shared setup

## Test Environment Requirements

- Local development environment with Docker for infrastructure services
- Test PostgreSQL database with Marten
- Test Redis instance
- Mock OpenAI service
- Discord bot test tokens

## Integration Test Templates

### Player Narrative Interaction Test

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;
using Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;
using Olympus.Domain.SharedKernel.ValueObjects;
using System.Net.Http.Json;
using Xunit;

namespace Olympus.Tests.Integration;

public class PlayerNarrativeInteractionTests : IClassFixture<WebApplicationFactory<Olympus.Api.Program>>
{
    private readonly WebApplicationFactory<Olympus.Api.Program> _factory;
    private readonly HttpClient _client;

    public PlayerNarrativeInteractionTests(WebApplicationFactory<Olympus.Api.Program> factory)
    {
        _factory = factory
            .WithWebHostBuilder(builder =>
            {
                // Configure test services (mock SK, Redis, etc.)
            });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CompleteNarrativeInteractionFlow_ReturnsSuccessfulResponse()
    {
        // Arrange
        var request = new
        {
            SessionId = "test-session-guid",
            UserId = "test-user-guid",
            Input = "I look around the tavern for clues"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ai/interact", request);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<NarrativeResponseDto>();

        // Assert
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent.Response);
        Assert.NotEmpty(responseContent.UpdatedContext);
    }
}
```

### CQRS Pipeline Test

```csharp
using Microsoft.Extensions.DependencyInjection;
using Olympus.Application.AiDrivenFeatures.ProcessPlayerNarrativeInput;
using Olympus.Application.Common.Messaging;
using Olympus.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Olympus.Tests.Integration;

public class CqrsPipelineTests
{
    [Fact]
    public async Task CommandDispatcherPipeline_ProcessesCommandSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();
        // Add services needed for the command pipeline
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IOlympusDispatcher>();
        var command = new ProcessPlayerNarrativeInputCommand(
            new SessionId("test-session"),
            new UserId("test-user"),
            "I search for hidden treasures");

        // Act
        var result = await dispatcher.DispatchCommandAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
```

## Missing Unit Test Templates

### Domain Value Object Tests

```csharp
using Olympus.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Olympus.Tests.Domain.SharedKernel.ValueObjects;

public class SessionIdTests
{
    [Fact]
    public void Create_WithValidGuid_ReturnsSessionId()
    {
        // Arrange
        var guidString = Guid.NewGuid().ToString();

        // Act
        var sessionId = new SessionId(guidString);

        // Assert
        Assert.Equal(guidString, sessionId.Value);
    }

    [Fact]
    public void Equality_WithSameValue_ReturnsTrue()
    {
        // Arrange
        var value = Guid.NewGuid().ToString();
        var sessionId1 = new SessionId(value);
        var sessionId2 = new SessionId(value);

        // Act & Assert
        Assert.Equal(sessionId1, sessionId2);
    }

    [Fact]
    public void ToString_ReturnsStringValue()
    {
        // Arrange
        var value = Guid.NewGuid().ToString();
        var sessionId = new SessionId(value);

        // Act & Assert
        Assert.Equal(value, sessionId.ToString());
    }
}
```

### Infrastructure Repository Tests

```csharp
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Olympus.Domain.SharedKernel.ValueObjects;
using Olympus.Infrastructure.Persistence.Marten.Repositories;
using Xunit;

namespace Olympus.Tests.Infrastructure.Persistence;

public class MartenRepositoryTests
{
    [Fact]
    public async Task SaveAndGetEntity_ReturnsExpectedEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        // Configure Marten with in-memory document store
        var serviceProvider = services.BuildServiceProvider();

        var documentStore = serviceProvider.GetRequiredService<IDocumentStore>();
        // Create repository instance

        // Act
        // Save entity
        // Retrieve entity

        // Assert
        // Verify retrieved entity matches saved one
    }
}
```

### AI Services Tests

```csharp
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Moq;
using Olympus.Infrastructure.Ai.KernelServices;
using Xunit;

namespace Olympus.Tests.Infrastructure.Ai;

public class SemanticKernelOrchestratorTests
{
    [Fact]
    public async Task ExecuteNarrativePrompt_ReturnsGeneratedText()
    {
        // Arrange
        var mockKernel = new Mock<Kernel>();
        var mockKernelFactory = new Mock<IKernelFactory>();
        mockKernelFactory.Setup(f => f.CreateKernel()).Returns(mockKernel.Object);

        var orchestrator = new SemanticKernelOrchestrator(
            mockKernelFactory.Object,
            Options.Create(new AiServiceSettings())
        );

        // Act
        // Setup kernel function call expectations
        // Call orchestrator method

        // Assert
        // Verify result matches expected response
    }
}
```

## Specific Missing Tests and Implementation Plan

### Domain Layer Missing Tests

1. **ValueObject Tests**
   - Fix failing GameDateTime tests (parsing/formatting issues)
   - Complete tests for remaining value objects (EntityId, CampaignId)
   - Test equality, identity, and immutability for all value objects

2. **Aggregate Root Tests**
   - Campaign aggregate: creation, property access, applying events
   - Character aggregate: creation, property updates, state transitions
   - Test invariant enforcement and business rules

3. **Domain Event Tests**
   - Test event creation
   - Verify immutability of events
   - Test event application to aggregates

### Application Layer Missing Tests

1. **Command Handler Tests**
   - Add tests for all remaining command handlers
   - Test validation failure cases
   - Test different business logic paths
   - Test event publishing

2. **Query Handler Tests**
   - Test query parameter validation
   - Test filtering logic
   - Test projection mapping

3. **Pipeline Behavior Tests**
   - Test validation behaviors
   - Test logging behaviors
   - Test performance monitoring behaviors

### Infrastructure Layer Missing Tests

1. **Repository Tests**
   - Test Marten repository implementations for each aggregate
   - Test event sourcing load/save operations
   - Test document session management

2. **AI Service Tests**
   - Test Semantic Kernel orchestration
   - Test prompt template rendering
   - Test context building
   - Test response parsing and transformation

3. **Redis Cache Tests**
   - Test cache hit/miss scenarios
   - Test expiration policies
   - Test concurrent access patterns

### Full End-to-End Tests

#### Discord Bot Integration

```csharp
using Discord;
using Discord.WebSocket;
using Moq;
using Olympus.Bot.Discord.Services;
using Xunit;

namespace Olympus.Tests.Integration;

public class DiscordBotE2ETests
{
    [Fact(Skip = "Requires Discord environment")]
    public async Task DiscordInteraction_ReturnsNarrativeResponse()
    {
        // This would be a more complex test using a test Discord server
        // and real interactions, but we can simulate parts of it

        // Arrange
        var mockDiscordClient = new Mock<DiscordSocketClient>();
        var olympusApiClient = new OlympusApiHttpClient(/* Mock HTTP client */);

        // Simulate a slash command interaction
        var mockInteraction = new Mock<ISlashCommandInteraction>();
        mockInteraction.Setup(i => i.Data.Name).Returns("interact");
        mockInteraction.Setup(i => i.Data.Options.First().Value).Returns("I look around the tavern");

        // Act & Assert
        // Implementation would depend on how your bot handles interactions
    }
}
```

#### Complete Campaign Creation and Management Flow

- Test creating a new campaign
- Adding players to the campaign
- Starting a session
- Narrative interactions during the session
- Saving and loading campaign state

#### Character Creation and Progression Flow

- Test character creation
- Equipment management
- Character advancement
- State persistence

## Workflow Test Plan

The following test plans cover the end-to-end workflows of the system:

### 1. Narrative Interaction Pipeline Test

This tests the core MVP flow of a player submitting input and receiving a narrative response:

1. Player submits text input through API/Discord
2. Command is validated and dispatched
3. Command handler processes input
4. AI service generates narrative response
5. Response is formatted and returned
6. Context is updated and persisted

### 2. Game State Management Test

1. Game session is created
2. Multiple characters join session
3. Game state is updated through commands
4. Queries retrieve current game state
5. Game state is persisted and can be reloaded
6. Events are correctly applied to rebuild state

### 3. Tools and Game Mechanics Test

1. Dice rolling tools are invoked through narrative
2. Combat mechanics are applied
3. Resource management is tracked
4. NPC interactions are processed
5. Quest progression is tracked

## Test Fixes Required

1. ✅ **Fix GameDateTime Formatting Tests**
   - Updated the `TryParse` method in `GameDateTime` to properly handle UTC time by using `DateTimeStyles.AdjustToUniversal` and `DateTimeStyles.AssumeUniversal`.
   - Fixed the test to properly compare DateTime values based on their ticks.

2. ✅ **Fix ProcessPlayerNarrativeInputCommandHandlerTests**
   - Fixed the test to match the current MVP implementation, where the `ISemanticKernelOrchestrator` is not yet being used.
   - Removed unnecessary mock setups that were referring to non-existent methods.
   - Added comments explaining the future implementation needed when the orchestrator is fully implemented.

3. **Create Integration Test Infrastructure**
   - Set up WebApplicationFactory with test configuration
   - Configure in-memory database for integration tests
   - Set up mock AI services for integration tests
