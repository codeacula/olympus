---
applyTo: "**/*.cs"
---
# C# Instructions

## 1. File Layout

```text
Usings (System first, then third‑party, then project‑local)
Namespace
File‑scoped namespace
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

## 8. Error Handling Implementation

- **Result Pattern Implementation**:

  ```csharp
  // Return success result
  return Result.Success(myValue);

  // Return failure result
  return Result.Failure(new Error("ErrorCode", "Error message"));

  // Chaining with Map
  return someResult
      .Map(value => TransformValue(value))
      .MapError(error => new UserFriendlyError(error));

  // Handling with pattern matching
  return myResult switch
  {
      { IsSuccess: true } => DoSomethingWithSuccess(myResult.Value),
      { IsFailure: true } => HandleFailure(myResult.Error),
      _ => throw new InvalidOperationException("Should not happen")
  };
  ```

- **Option Pattern Implementation**:

  ```csharp
  // Creating options
  var someOption = Option.Some(myValue);
  var noneOption = Option<MyType>.None;

  // Handling with Match
  return myOption.Match(
      some: value => ProcessValue(value),
      none: () => DefaultValue
  );

  // Converting from nullable
  public Option<User> GetUser(string id)
  {
      var user = _repository.FindById(id); // may return null
      return user != null ? Option.Some(user) : Option<User>.None;
  }
  ```

## 9. Domain Event Implementation

- **Event Structure**:

  ```csharp
  public record PlayerJoinedGameEvent(
      Guid GameId,                // Aggregate ID
      Guid PlayerId,              // Essential event data
      string PlayerName,
      DateTimeOffset OccurredAt,  // Timestamp
      int Version = 1)            // Schema version
  {
      // Adding methods to help with versioning if needed
      public PlayerJoinedGameEvent UpgradeToV2() =>
          this with { Version = 2 }; // Example of version upgrade
  }
  ```

- **Using Events with Aggregates**:

  ```csharp
  // In Game aggregate
  public void JoinGame(Player player)
  {
      if (_players.Any(p => p.Id == player.Id))
      {
          throw new DomainException("Player already joined");
      }

      var @event = new PlayerJoinedGameEvent(
          Id,
          player.Id,
          player.Name,
          _dateTimeProvider.Now);

      Apply(@event);
      AddEvent(@event);
  }

  private void Apply(PlayerJoinedGameEvent @event)
  {
      _players.Add(new PlayerInfo(@event.PlayerId, @event.PlayerName));
  }
  ```

## 10. Async/Await Best Practices

- **ConfigureAwait Usage**:

  ```csharp
  // For library code or reusable components, use ConfigureAwait(false)
  await _repository.GetAsync(id, token).ConfigureAwait(false);

  // For application/UI code, ConfigureAwait(true) is implied and can be omitted
  await _repository.GetAsync(id, token);
  ```

- **Async Method Naming**:

  ```csharp
  // All async methods should have the "Async" suffix
  public async Task<User> GetUserAsync(string id, CancellationToken token)

  // Exception: Event handlers, test methods with [Fact]/[Theory] attributes
  ```

- **Cancellation Token Propagation**:

  ```csharp
  // Always include CancellationToken parameters in async methods
  // and propagate them to other async calls
  public async Task<Result<User, Error>> GetUserAsync(
      string id,
      CancellationToken token)
  {
      return await _repository.GetUserAsync(id, token);
  }
  ```

## 11. Nullable Reference Types

- **Nullability Annotations**:

  ```csharp
  // Use nullable reference type annotations appropriately
  public User? FindById(string id)  // Explicitly nullable return

  // For parameters that shouldn't be null, don't use ? and validate:
  public void ProcessUser(User user) // Non-nullable parameter
  {
      // Optional: Add runtime validation if needed
      if (user is null) throw new ArgumentNullException(nameof(user));
      // ...
  }
  ```

- **Null Checks**:

  ```csharp
  // For APIs that don't use Option<T>, perform null checks early
  public Result<UserDto, Error> Process(UserCommand cmd)
  {
      if (cmd.UserId is null)
      {
          return Result.Failure(new Error("InvalidCommand", "UserId cannot be null"));
      }
      // ...
  }
  ```

## 12. CQRS Query Examples

- **Query Definition**:

  ```csharp
  namespace Olympus.Application.Characters.Queries.GetCharacterDetails;

  public sealed record GetCharacterDetailsQuery(
      Guid CharacterId) : IRequest<Result<CharacterDetailsDto, Error>>;

  internal sealed class Handler : IRequestHandler<GetCharacterDetailsQuery,
                                                 Result<CharacterDetailsDto, Error>>
  {
      private readonly ICharacterReadModelRepository _repository;
      private readonly ILogger<Handler> _logger;

      public Handler(
          ICharacterReadModelRepository repository,
          ILogger<Handler> logger)
      {
          _repository = repository;
          _logger = logger;
      }

      public async Task<Result<CharacterDetailsDto, Error>> Handle(
          GetCharacterDetailsQuery query,
          CancellationToken token)
      {
          var character = await _repository.GetByIdAsync(query.CharacterId, token);

          if (character is null)
          {
              _logger.LogWarning("Character {CharacterId} not found", query.CharacterId);
              return Result.Failure(new Error("NotFound", "Character not found"));
          }

          return Result.Success(new CharacterDetailsDto(
              character.Id,
              character.Name,
              character.Level,
              character.Classes));
      }
  }
  ```

## 13. XML Documentation

- **Public API Documentation**:

  ```csharp
  /// <summary>
  /// Represents a player character in the game world.
  /// </summary>
  public class Character
  {
      /// <summary>
      /// Attempts to level up the character if prerequisites are met.
      /// </summary>
      /// <param name="experiencePoints">The amount of XP to apply toward leveling.</param>
      /// <returns>A result containing the new level if successful, or an error.</returns>
      /// <exception cref="ArgumentOutOfRangeException">
      /// Thrown when <paramref name="experiencePoints"/> is negative.
      /// </exception>
      public Result<int, Error> TryLevelUp(int experiencePoints)
      {
          // Implementation
      }
  }
  ```

## 14. References

- See [general-coding.instructions.md](general-coding.instructions.md) for overarching principles.
