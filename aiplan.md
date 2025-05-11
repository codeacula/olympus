Okay, Code, let's get these core components of the **initial narrative interaction loop** detailed! This will be the engine for your "ASAP AI Narrative Campaign."

We'll focus on:

1. The `GameMasterPlugin` (specifically its main semantic function and a simple native tool).
2. The `IGameSessionNarrativeContextService` for short-term memory.
3. The `ProcessPlayerNarrativeInputCommandHandler` that ties it all together.

---

### 1. Semantic Kernel Plugin: `GameMasterPlugin`

This plugin will be responsible for generating the AI GM's narrative responses.

* **Location:** `Olympus.Infrastructure.Ai/Plugins/GameMasterPlugin/`

#### a. Semantic Function: `GeneratePlayerTurnOutcome`

This is the core prompt that drives the AI GM's response.

* **Folder:** `Olympus.Infrastructure.Ai/Plugins/GameMasterPlugin/GeneratePlayerTurnOutcome/`
* **`skprompt.txt`:**

    ```
    System:
    You are "Olympus," a skilled and imaginative Game Master for a narrative-focused tabletop roleplaying game.
    Your primary goal is to weave an engaging, interactive story based on player actions and dialogue.
    Prioritize descriptive narration, believable NPC interactions (if any are present or implied by the player), and creating opportunities for continued play.
    For now, avoid complex game mechanics or dice rolls unless specifically invoked by a tool.
    If a player's action has an uncertain outcome where a simple element of chance would add narrative flavor (e.g., "I try to subtly persuade the guard," "I search the dusty bookshelf for a hidden lever"), you can use the "SimpleChanceRoll_Evaluate" tool.
    When using the "SimpleChanceRoll_Evaluate" tool, incorporate its qualitative result (e.g., 'Success', 'Partial Success', 'Failure') seamlessly into your narrative description of the outcome. Do not mention dice or numbers.
    Conclude your response by setting the scene for the player's next action, perhaps by describing a new situation or posing a question.

    User Context:
    Current Scene: {{sceneDescription}}
    Player Character: {{characterPersona}}
    {{#if recentHistory}}
    Recent Interaction History (last few exchanges, most recent first):
    {{#each recentHistory}}
    - {{this.author}}: {{this.content}}
    {{/each}}
    {{/if}}

    User:
    {{playerInput}}

    Assistant (Olympus GM):
    ```

* **`config.json`:**

    ```json
    {
      "schema": 1,
      "type": "completion",
      "description": "Generates a Game Master narrative response to a player's input, considering scene, persona, and history.",
      "completion": {
        "max_tokens": 500, // Adjust as needed
        "temperature": 0.7,
        "top_p": 0.9,
        "presence_penalty": 0.1,
        "frequency_penalty": 0.1
      },
      "input_variables": [
        { "name": "sceneDescription", "description": "A brief description of the current game scene.", "required": true },
        { "name": "characterPersona", "description": "A brief description of the player character.", "required": true },
        { "name": "recentHistory", "description": "A short list of recent player/GM exchanges (optional).", "required": false },
        { "name": "playerInput", "description": "The player's natural language input (action or dialogue).", "required": true }
      ],
      "feature_filters": {
        "plugins": ["CoreUtilityPlugin"] // Ensure SimpleChanceRoll (if part of CoreUtilityPlugin) is available
      }
    }
    ```

#### b. Native Function (Tool): `SimpleChanceRoll`

This provides a basic chance mechanism for the LLM. It will live in a more general utility plugin.

* **Plugin & Location:** `Olympus.Infrastructure.Ai/Plugins/CoreUtilityPlugin/NativeFunctions/SimpleChanceRoll.cs`
* **`SimpleChanceRoll.cs`:**

    ```csharp
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Microsoft.SemanticKernel;

    namespace Olympus.Infrastructure.Ai.Plugins.CoreUtilityPlugin.NativeFunctions;

    public class SimpleChanceRoll
    {
        private readonly Random _random = new();

        [KernelFunction, Description("Evaluates a simple, narrative-focused chance for a described player action. Returns 'Critical Success', 'Success', 'Partial Success', 'Failure', or 'Critical Failure'.")]
        public Task<string> EvaluateAsync(
            [Description("A brief description of the action whose chance is being evaluated (e.g., 'persuading the guard', 'searching the desk thoroughly').")]
            string actionDescription // The description can be used for logging or future context, though not directly in this simple roll.
        )
        {
            // Simple d100 style roll for qualitative outcome
            int roll = _random.Next(1, 101);
            string outcome;

            if (roll <= 5) outcome = "Critical Failure";       // 5%
            else if (roll <= 30) outcome = "Failure";          // 25%
            else if (roll <= 70) outcome = "Partial Success";  // 40%
            else if (roll <= 95) outcome = "Success";          // 25%
            else outcome = "Critical Success";                 // 5%

            // Potentially log: $"SimpleChanceRoll for '{actionDescription}': {roll} -> {outcome}"
            return Task.FromResult(outcome);
        }
    }
    ```

  * **Note:** This `CoreUtilityPlugin` (containing `SimpleChanceRoll` and potentially the `DiceRoller` we discussed earlier) would need to be registered with the Semantic Kernel `Kernel` instance by the `KernelFactory`.

---

### 2. `IGameSessionNarrativeContextService` (Short-Term Memory for AI)

This service manages the brief conversational history and simple scene notes for an ongoing game session to provide context to the LLM.

* **Interface Location:** `Olympus.Application/Abstractions/Ai/IGameSessionNarrativeContextService.cs`
* **`IGameSessionNarrativeContextService.cs`:**

    ```csharp
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Olympus.Application.Common.Types; // For Result & Option
    using Olympus.Domain.ValueObjects.StronglyTypedIDs; // For SessionId if you make one

    namespace Olympus.Application.Abstractions.Ai;

    public record NarrativeExchange(string Author, string Content, DateTimeOffset Timestamp);

    public record NarrativeContext(
        SessionId SessionId, // Assuming SessionId is a strongly-typed ID
        string CurrentSceneDescription, // Simple text description
        string CharacterPersona,        // Simple text description
        IReadOnlyList<NarrativeExchange> RecentHistory // Last N exchanges
    );

    public interface IGameSessionNarrativeContextService
    {
        Task<Result<Option<NarrativeContext>, Error>> GetContextAsync(SessionId sessionId, CancellationToken cancellationToken = default);

        Task<Result<Success, Error>> UpdateContextAsync(
            SessionId sessionId,
            string newPlayerInput,
            string newGmResponse,
            string? updatedSceneDescription = null, // Optional: if the scene changes
            string? updatedCharacterPersona = null, // Optional: if persona evolves slightly
            CancellationToken cancellationToken = default);

        Task<Result<Success, Error>> InitializeContextAsync(
            SessionId sessionId,
            string initialSceneDescription,
            string initialCharacterPersona,
            CancellationToken cancellationToken = default);
    }
    ```

    *(Note: `SessionId` would be a `readonly record struct SessionId(Guid Value);` defined in Domain/ValueObjects or a shared kernel if applicable across domains. For simplicity, you could start with `string sessionId`)*

* **Implementation Outline (`Olympus.Infrastructure.Caching.Redis/Services/RedisGameSessionNarrativeContextService.cs`):**
  * **Injects:** `IConnectionMultiplexer` (StackExchange.Redis).
  * **`InitializeContextAsync`**: Creates a Redis Hash for the `sessionId`. Stores `sceneDescription` and `characterPersona` as fields. Initializes an empty Redis List for `recentHistory`.
  * **`GetContextAsync`**:
    * Reads the `sceneDescription` and `characterPersona` fields from the session's Hash.
    * Reads the last N items (e.g., 5-10) from the session's `recentHistory` List.
    * Deserializes `NarrativeExchange` objects from the list.
    * Constructs and returns the `NarrativeContext`. If not found, returns `Option<NarrativeContext>.None`.
  * **`UpdateContextAsync`**:
    * Creates `NarrativeExchange` for player input and GM response.
    * LPUSHes these (serialized as JSON) to the `recentHistory` List for the `sessionId`.
    * LTRIMs the list to keep it to a maximum size (e.g., trim to keep only the latest 10-20 exchanges).
    * If `updatedSceneDescription` or `updatedCharacterPersona` are provided, updates those fields in the session's Hash.
  * **Error Handling:** Wraps Redis operations in try-catch, returning appropriate `Error` objects on failure.
  * **Serialization:** Uses `System.Text.Json` for serializing `NarrativeExchange` before storing in Redis Lists.

---

### 3. `ProcessPlayerNarrativeInputCommandHandler` (Orchestrator)

This application service handler ties the player input, context, and AI call together.

* **Location:** `Olympus.Application/AiDrivenFeatures/Commands/ProcessPlayerNarrativeInput/`
* **`ProcessPlayerNarrativeInputCommandHandler.cs` (Key Logic):**

    ```csharp
    using MediatR;
    using Microsoft.Extensions.Logging; // For logging
    using Olympus.Application.Abstractions.Ai;
    using Olympus.Application.Common.Types; // For Result, Success, Error
    using Microsoft.SemanticKernel; // For KernelArguments
    using Olympus.Domain.ValueObjects.StronglyTypedIDs; // For SessionId

    namespace Olympus.Application.AiDrivenFeatures.Commands.ProcessPlayerNarrativeInput;

    // Command record (defined in ProcessPlayerNarrativeInputCommand.cs)
    // public record ProcessPlayerNarrativeInputCommand(
    //     SessionId SessionId,
    //     UserId PlayerId, // Assuming a UserId value object
    //     string PlayerInputText
    // ) : IRequest<Result<NarrativeResponseDto, Error>>;

    // Response DTO (defined in Application/AiDrivenFeatures/DTOs/)
    // public record NarrativeResponseDto(string GmTextOutput);

    public class ProcessPlayerNarrativeInputCommandHandler
        : IRequestHandler<ProcessPlayerNarrativeInputCommand, Result<NarrativeResponseDto, Error>>
    {
        private readonly ISemanticKernelOrchestrator _kernelOrchestrator;
        private readonly IGameSessionNarrativeContextService _contextService;
        private readonly ILogger<ProcessPlayerNarrativeInputCommandHandler> _logger;

        public ProcessPlayerNarrativeInputCommandHandler(
            ISemanticKernelOrchestrator kernelOrchestrator,
            IGameSessionNarrativeContextService contextService,
            ILogger<ProcessPlayerNarrativeInputCommandHandler> logger)
        {
            _kernelOrchestrator = kernelOrchestrator;
            _contextService = contextService;
            _logger = logger;
        }

        public async Task<Result<NarrativeResponseDto, Error>> Handle(
            ProcessPlayerNarrativeInputCommand command,
            CancellationToken cancellationToken)
        {
            // 1. Get current narrative context for the session
            var contextResult = await _contextService.GetContextAsync(command.SessionId, cancellationToken);
            if (contextResult.IsFailure) return contextResult.Error;
            if (contextResult.Value.IsNone)
            {
                // Context not found; perhaps initialize it or return an error.
                // For "ASAP", let's assume it should be initialized beforehand or handle this gracefully.
                // For now, returning an error if context is missing. A real game would have a scene setup flow.
                _logger.LogWarning("Narrative context not found for session {SessionId}", command.SessionId);
                // For now, let's use a default if none found, or initialize it here
                // This part needs a flow for initializing a scene/session context.
                // For this example, we'll mock a simple one if not found.
                var narrativeContext = contextResult.Value.ValueOr(
                    new NarrativeContext(command.SessionId, "A mysterious, dimly lit cave.", "A curious adventurer.", new List<NarrativeExchange>())
                ); 
                // Consider calling _contextService.InitializeContextAsync here if contextResult.Value.IsNone
                // and then re-fetching, or designing InitializeContextAsync to return the context.
            }
            else
            {
                 narrativeContext = contextResult.Value.Value;
            }


            // 2. Prepare arguments for Semantic Kernel
            var kernelArgs = new KernelArguments
            {
                ["playerInput"] = command.PlayerInputText,
                ["sceneDescription"] = narrativeContext.CurrentSceneDescription,
                ["characterPersona"] = narrativeContext.CharacterPersona,
                ["recentHistory"] = narrativeContext.RecentHistory // SK prompt can handle if this is null/empty
            };

            // 3. Invoke the AI GM prompt function
            var generationResult = await _kernelOrchestrator.InvokePromptFunctionAsync(
                "NarrativeGmPlugin",
                "GeneratePlayerTurnOutcome",
                kernelArgs,
                cancellationToken);

            if (generationResult.IsFailure)
            {
                _logger.LogError("AI generation failed for session {SessionId}: {Error}", command.SessionId, generationResult.Error);
                return generationResult.Error; // Propagate the error
            }

            string gmResponseText = generationResult.Value.Text; // Assuming GeneratedTextResponse has a Text property

            // 4. Update the narrative context with the new exchange
            var updateResult = await _contextService.UpdateContextAsync(
                command.SessionId,
                command.PlayerInputText,
                gmResponseText,
                // We could also allow the AI to suggest sceneDescription changes if the prompt supports it
                // and then pass that back here. For now, keeping it simple.
                cancellationToken: cancellationToken
            );

            if (updateResult.IsFailure)
            {
                // Log the context update failure but proceed with returning the GM response,
                // as the core interaction succeeded. Or decide if this is critical.
                _logger.LogWarning("Failed to update narrative context for session {SessionId}: {Error}", command.SessionId, updateResult.Error);
            }

            // 5. Return the GM's response
            return new NarrativeResponseDto(gmResponseText);
        }
    }
    ```

---

This detailed breakdown provides the core structure and logic for your initial AI-driven narrative loop.

* The `GameMasterPlugin` with its prompt and the `SimpleChanceRoll` tool forms the AI's "brain."
* The `IGameSessionNarrativeContextService` provides its short-term memory.
* The `ProcessPlayerNarrativeInputCommandHandler` is the conductor making them work together.

This should give you a very solid starting point to begin implementing this "ASAP" feature!
