using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Olympus.Application.AiDrivenFeatures.Common.DTOs;

namespace Olympus.Tests.Integration;

public class PlayerNarrativeInteractionTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
{
  [Fact]
  public async Task CompleteNarrativeInteractionFlow_ReturnsValidNarrativeResponseAsync()
  {
    // Arrange
    const string sessionId = "test-session-guid";
    const string userId = "test-user-guid";
    const string userInput = "I look around the tavern for clues";

    var request = new
    {
      SessionId = sessionId,
      UserId = userId,
      Input = userInput
    };

    // Act
    var response = await Client.PostAsJsonAsync("/api/ai/interact", request);

    // Assert
    _ = response.EnsureSuccessStatusCode();
    var responseContent = await response.Content.ReadFromJsonAsync<NarrativeResponseDto>();

    Assert.NotNull(responseContent);
    Assert.NotNull(responseContent!.Response);
    Assert.NotEmpty(responseContent.UpdatedContext);

    // Additional assertions for robustness
    Assert.Contains("Mock response", responseContent.Response);
    Assert.All(responseContent.UpdatedContext, contextItem => Assert.False(string.IsNullOrWhiteSpace(contextItem)));
  }
}
