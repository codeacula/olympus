namespace Olympus.Application.Ai.Services;

public interface IOpenAiService
{
  Task<IOlympusResult<TestAiInteractionCommandResult, OlympusError>> TestInteractionAsync(
      string interactionText,
      CancellationToken cancellationToken = default);
}
