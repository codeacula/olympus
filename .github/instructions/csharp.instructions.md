---
applyTo: "**/*.cs"
---

## 1. File Layout

```text
Namespace
Usings (System first, then third‑party, then project‑local)
File‑scope namespace
Type declarations

```

## 2. Records for DTOs
    ```csharp
    namespace Olympus.Application.Characters.DTOs;

    public sealed record CharacterSummaryDto(
        Guid Id,
        string Name,
        int Level,
        IReadOnlyList<string> Classes);
    ```

## 3. Commands and Handlers
    ```csharp
    namespace Olympus.Application.AiDrivenFeatures.Commands.ProcessPlayerNarrativeInput;

    public sealed record ProcessPlayerNarrativeInputCommand(
        Guid SessionId,
        Guid UserId,
        string InputText) : IRequest<Result<NarrativeResponseDto, Error>>;

    internal sealed class Handler : IRequestHandler<ProcessPlayerNarrativeInputCommand,
                                                    Result<NarrativeResponseDto, Error>>
    {
        private readonly ISemanticKernelOrchestrator _sk;
        private readonly IGameSessionNarrativeContextService _ctx;
        private readonly ILogger<Handler> _log;

        public Handler(ISemanticKernelOrchestrator sk,
                       IGameSessionNarrativeContextService ctx,
                       ILogger<Handler> log)
        {
            _sk  = sk;
            _ctx = ctx;
            _log = log;
        }

        public async Task<Result<NarrativeResponseDto, Error>> Handle(
            ProcessPlayerNarrativeInputCommand cmd,
            CancellationToken token)
        {
            using var _ = _log.BeginScope("Session {SessionId}", cmd.SessionId);

            var ctx = await _ctx.LoadAsync(cmd.SessionId, token);

            var response = await _sk.GenerateAsync(ctx, cmd.InputText, token);

            await _ctx.SaveAsync(cmd.SessionId, ctx with { LastExchange = response }, token);

            return Result.Success(response);
        }
    }
    ```

## 4. Repository Example
    ```csharp
    public interface ICharacterRepository
    {
        Task<Character?> GetAsync(CharacterId id, CancellationToken token);
        Task SaveAsync(Character aggregate, CancellationToken token);
    }
    ```

## 5. Logging
    ```csharp
    public static partial class CharacterLog
    {
        [LoggerMessage(Level = LogLevel.Information,
                       Message = "Character {CharacterId} leveled up to {Level}")]
        public static partial void CharacterLeveledUp(
            ILogger logger, Guid characterId, int level);
    }
    ```

## 6. DTO vs DBO
- **DTO**: external contract, immutable record in `Olympus.Application`.
- **DBO**: persistence shape used by Marten, may include internal IDs.

## 7. ECS Component Sample
    ```csharp
    public readonly record struct PositionComponent(float X, float Y, float Z);
    ```

## 8. References
- See [general-coding.instructions.md](general-coding.instructions.md) for overarching principles.